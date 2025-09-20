using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    public AudioMixer audioMixer;                  // arraste o Game_AudioMixer aqui
    public AudioMixerGroup musicGroup;             // arraste Music group
    public AudioMixerGroup sfxGroup;               // arraste SFX group

    // Exposed parameter names (deixe iguais aos que expôs no Mixer)
    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM = "SFXVolume";

    [Header("Music")]
    public float defaultMusicVolume = 1f;          // 0..1
    public int sfxPoolSize = 12;

    // Duas fontes para crossfade suave
    private AudioSource musicSourceA;
    private AudioSource musicSourceB;
    private bool usingA = true;
    private Coroutine musicFadeCoroutine;

    [Header("SFX pool")]
    private List<AudioSource> sfxPool;

    [Header("Optional Sound Bank (assign in inspector)")]
    public SFXEntry[] sfxEntries; // preencher com nome->clip no inspector
    private Dictionary<string, AudioClip> sfxBank;

    [System.Serializable]
    public struct SFXEntry { public string key; public AudioClip clip; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // --- Music sources ---
        musicSourceA = gameObject.AddComponent<AudioSource>();
        musicSourceB = gameObject.AddComponent<AudioSource>();

        musicSourceA.outputAudioMixerGroup = musicGroup;
        musicSourceB.outputAudioMixerGroup = musicGroup;

        musicSourceA.loop = true;
        musicSourceB.loop = true;

        musicSourceA.spatialBlend = 0f;
        musicSourceB.spatialBlend = 0f;

        musicSourceA.playOnAwake = false;
        musicSourceB.playOnAwake = false;

        // --- SFX pool ---
        sfxPool = new List<AudioSource>();
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = sfxGroup;
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f; // 2D
            sfxPool.Add(src);
        }

        // --- bank ---
        sfxBank = new Dictionary<string, AudioClip>();
        if (sfxEntries != null)
        {
            foreach (var e in sfxEntries)
                if (!string.IsNullOrEmpty(e.key) && e.clip != null)
                    sfxBank[e.key] = e.clip;
        }

        // Aplica valores iniciais salvos (PlayerPrefs)
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        float savedSfx = PlayerPrefs.GetFloat("VFXVolume", 1f);
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSfx);
    }

    // ---------------------------
    // Mixer volume helpers (linear 0..1 -> dB)
    private float LinearToDb(float linear)
    {
        if (linear <= 0f) return -80f; // só no 0 absoluto
        return 20f * Mathf.Log10(linear);
    }

    public void SetMusicVolume(float value)
    {
        float dB = LinearToDb(value);
        if (audioMixer != null) audioMixer.SetFloat(MUSIC_PARAM, dB);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        float dB = LinearToDb(value);
        if (audioMixer != null) audioMixer.SetFloat(SFX_PARAM, dB);
        PlayerPrefs.SetFloat("VFXVolume", value);
        PlayerPrefs.Save();
    }

    // ---------------------------
    // Music control (crossfade)
    public void PlayMusic(AudioClip clip, float crossfade = 0.5f)
    {
        if (clip == null) return;

        // se nenhum tocando ainda, toca diretamente
        AudioSource active = usingA ? musicSourceA : musicSourceB;
        AudioSource other = usingA ? musicSourceB : musicSourceA;

        if (!active.isPlaying && !other.isPlaying)
        {
            active.clip = clip;
            active.volume = 1f;
            active.Play();
            usingA = !usingA;
            return;
        }

        // crossfade: new -> other, fade duration
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(CrossfadeMusicCoroutine(clip, crossfade));
    }

    public void StopMusic(float fadeOut = 0.5f)
    {
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        StartCoroutine(FadeStopAllMusic(fadeOut));
    }

    private IEnumerator CrossfadeMusicCoroutine(AudioClip newClip, float dur)
    {
        AudioSource oldSrc = usingA ? musicSourceA : musicSourceB;
        AudioSource newSrc = usingA ? musicSourceB : musicSourceA;

        newSrc.clip = newClip;
        newSrc.volume = 0f;
        newSrc.Play();

        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float a = t / dur;
            oldSrc.volume = Mathf.Lerp(1f, 0f, a);
            newSrc.volume = Mathf.Lerp(0f, 1f, a);
            yield return null;
        }

        oldSrc.Stop();
        newSrc.volume = 1f;
        usingA = !usingA;
        musicFadeCoroutine = null;
    }

    private IEnumerator FadeStopAllMusic(float dur)
    {
        AudioSource a = musicSourceA;
        AudioSource b = musicSourceB;

        float startA = a.isPlaying ? a.volume : 0f;
        float startB = b.isPlaying ? b.volume : 0f;
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float aL = Mathf.Lerp(startA, 0f, t / dur);
            float bL = Mathf.Lerp(startB, 0f, t / dur);
            a.volume = aL;
            b.volume = bL;
            yield return null;
        }
        if (a.isPlaying) a.Stop();
        if (b.isPlaying) b.Stop();
    }

    // ---------------------------
    // SFX
    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        foreach (var src in sfxPool)
        {
            if (!src.isPlaying)
            {
                src.clip = clip;
                src.volume = Mathf.Clamp01(volume);
                src.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
                src.Play();
                return;
            }
        }
        // fallback
        sfxPool[0].PlayOneShot(clip, volume);
    }

    public void PlaySFXRandomPitch(AudioClip clip, float volume = 1f, float pitchVariance = 0.1f)
    {
        float pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
        PlaySFX(clip, volume, pitch);
    }

    public void PlaySFXByKey(string key, float volume = 1f)
    {
        if (sfxBank != null && sfxBank.TryGetValue(key, out var clip))
            PlaySFX(clip, volume);
    }

    // Optionally: PlaySFX at animation event by passing AudioClip param:
    // public void PlaySFXEvent(AudioClip clip) { PlaySFX(clip); }
}
