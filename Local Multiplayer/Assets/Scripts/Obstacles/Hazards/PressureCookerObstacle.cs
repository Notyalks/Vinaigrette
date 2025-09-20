using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D), typeof(Animator))]
public class PressureCookerObstacle : TimedObstacleBase
{
    [Header("Configuração do Vapor")]
    public float verticalForce = 25f;
    public float horizontalForce = 100f;

    [Header("Som do Vapor")]
    public AudioClip steamClip;             // Som contínuo de vapor
    public float endFadeoutDuration = 1.5f; // Tempo de fade quando acaba
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    private bool isPushing = false;
    private bool atMaxForce = false;
    private List<PlayerMovementBase> playersInside = new List<PlayerMovementBase>();
    private Animator anim;
    private Collider2D col;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = AudioManager.Instance?.sfxGroup;
        audioSource.playOnAwake = false;
        audioSource.loop = true;   // 🔄 som de vapor em loop
        audioSource.spatialBlend = 0f; // 2D
    }

    protected override void OnInterval()
    {
        anim.SetTrigger("Steam");
    }

    // =====================
    // EVENTOS DA ANIMAÇÃO
    // =====================
    public void StartSteamPush()
    {
        isPushing = true;
        atMaxForce = false;

        foreach (var player in playersInside)
            if (player != null)
                player.BlockInputs(true);

        PlaySteamSFX();
    }

    public void BlockSteamArea()
    {
        atMaxForce = true;
    }

    public void EndSteamPush()
    {
        isPushing = false;
        atMaxForce = false;

        foreach (var player in playersInside)
            if (player != null)
                player.BlockInputs(false);

        PlaySteamEndSFX();
    }

    // =====================
    // EMPURRÃO
    // =====================
    private void FixedUpdate()
    {
        if (!isPushing) return;

        Vector2 dir = transform.up.normalized;
        float baseForce = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? horizontalForce : verticalForce;
        float currentForce = atMaxForce ? baseForce : baseForce * 0.5f;

        foreach (var player in playersInside)
        {
            if (player != null)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    float dot = Vector2.Dot(rb.linearVelocity, dir);
                    if (dot < 0)
                        rb.linearVelocity -= dir * dot;

                    rb.AddForce(dir * currentForce, ForceMode2D.Force);
                }
            }
        }
    }

    // =====================
    // DETECÇÃO DE PLAYERS
    // =====================
    private void OnTriggerEnter2D(Collider2D other)
    {
        var move = other.GetComponent<PlayerMovementBase>();
        if (move != null && !playersInside.Contains(move))
            playersInside.Add(move);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var move = other.GetComponent<PlayerMovementBase>();
        if (move != null)
        {
            move.BlockInputs(false);
            playersInside.Remove(move);
        }
    }

    // =====================
    // ÁUDIO
    // =====================
    public void PlaySteamSFX()
    {
        if (steamClip != null && audioSource != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

            audioSource.Stop();
            audioSource.clip = steamClip;
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }

    public void PlaySteamEndSFX()
    {
        if (audioSource == null || !audioSource.isPlaying)
            return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOutSteam());
    }

    private IEnumerator FadeOutSteam()
    {
        float startVol = audioSource.volume;
        float t = 0f;

        while (t < endFadeoutDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, t / endFadeoutDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 1f;
    }
}
