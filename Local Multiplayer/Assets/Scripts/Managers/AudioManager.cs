using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gerenciador de áudio centralizado (singleton).
/// Controla música de fundo e efeitos sonoros.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixers")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Sources")]
    private AudioSource musicSource;  // loop de música
    private AudioSource sfxSource;    // SFX únicos
    private List<AudioSource> sfxPool; // para múltiplos sons simultâneos

    [Header("Configuração")]
    public int sfxPoolSize = 10;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cria fontes de música e SFX
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();

        // Pool de SFX para sons sobrepostos
        sfxPool = new List<AudioSource>();
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            sfxPool.Add(src);
        }

        ApplyVolume();
    }

    // ---------------------------
    // Volume
    // ---------------------------
    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyVolume();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        foreach (var src in sfxPool)
            src.volume = sfxVolume;
    }

    // ---------------------------
    // Música
    // ---------------------------
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }

    // ---------------------------
    // Efeitos sonoros
    // ---------------------------
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        // pega um áudio do pool livre
        foreach (var src in sfxPool)
        {
            if (!src.isPlaying)
            {
                src.clip = clip;
                src.Play();
                return;
            }
        }

        // se nenhum livre, usa o principal
        sfxSource.PlayOneShot(clip);
    }
}
