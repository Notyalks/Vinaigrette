using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PressureCookerObstacle : TimedObstacleBase
{
    [Header("Configuração do Vapor")]
    public float pushForce = 15f;          // Força de empurrão do vapor
    public float pushDuration = 0.5f;      // Quanto tempo o jato dura
    public ParticleSystem steamEffect;     // Partículas de vapor (opcional)

    private bool isPushing = false;
    private List<Rigidbody2D> affectedPlayers = new List<Rigidbody2D>();

    protected override void OnInterval()
    {
        if (!isPushing)
            StartCoroutine(DoSteamBlast());
    }

    private IEnumerator DoSteamBlast()
    {
        isPushing = true;

        // Rotaciona o sistema de partículas na direção do empurrão
        if (steamEffect != null)
        {
            steamEffect.transform.rotation = transform.rotation;
            steamEffect.Play();
        }

        float timer = 0f;
        while (timer < pushDuration)
        {
            // Calcula a direção dinâmica baseada na rotação do objeto
            Vector2 dynamicDirection = transform.up;

            foreach (var rb in affectedPlayers)
            {
                if (rb != null)
                    rb.AddForce(dynamicDirection.normalized * pushForce, ForceMode2D.Force);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (steamEffect != null)
            steamEffect.Stop();

        isPushing = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (!affectedPlayers.Contains(rb))
                affectedPlayers.Add(rb);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
            affectedPlayers.Remove(player.GetComponent<Rigidbody2D>());
    }
}
