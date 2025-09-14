using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D), typeof(Animator))]
public class PressureCookerObstacle : TimedObstacleBase
{
    [Header("Configuração do Vapor")]
    public float verticalForce = 25f;   // força padrão quando para cima/baixo
    public float horizontalForce = 100f; // força quando para esquerda/direita

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

        // bloqueia inputs
        foreach (var player in playersInside)
        {
            if (player != null)
                player.BlockInputs(true);
        }
    }

    public void BlockSteamArea()
    {
        atMaxForce = true;
    }

    public void EndSteamPush()
    {
        isPushing = false;
        atMaxForce = false;

        // libera inputs
        foreach (var player in playersInside)
        {
            if (player != null)
                player.BlockInputs(false);
        }
    }

    // =====================
    // EMPURRÃO
    // =====================
    private void FixedUpdate()
    {
        if (!isPushing) return;

        // Direção do jato
        Vector2 dir = transform.up.normalized;

        // Define força baseada na orientação
        float baseForce = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? horizontalForce : verticalForce;
        float currentForce = atMaxForce ? baseForce : baseForce * 0.5f;

        foreach (var player in playersInside)
        {
            if (player != null)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Remove componente contrária (se o player estiver competindo contra o jato)
                    float dot = Vector2.Dot(rb.velocity, dir);
                    if (dot < 0)
                    {
                        // Cancela a velocidade contrária
                        rb.velocity -= dir * dot;
                    }

                    // Aplica força do vapor
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
            move.BlockInputs(false); // garante que inputs liberam ao sair
            playersInside.Remove(move);
        }
    }
}
