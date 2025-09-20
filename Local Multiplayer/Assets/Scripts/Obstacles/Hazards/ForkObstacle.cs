using UnityEngine;

/// <summary>
/// Obstáculo "Garfo" que sobe e desce em intervalos, causando dano como espinhos.
/// </summary>
[RequireComponent(typeof(Animator), typeof(Collider2D))]
public class ForkObstacle : TimedObstacleBase
{
    private Animator anim;
    private Collider2D hitbox;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        hitbox = GetComponent<Collider2D>();
        hitbox.enabled = false; // começa desativado (garfo abaixado)
    }

    // Chamado pelo TimedObstacleBase em cada intervalo.
    protected override void OnInterval()
    {
        // Dispara a animação que alterna entre subir/abaixar
        if (anim != null)
            anim.SetTrigger("Toggle");
    }

    // Chamado pela animação quando o garfo chega na posição de ataque.
    public void EnableCollider()
    {
        if (hitbox != null)
            hitbox.enabled = true;
    }

    // Chamado pela animação quando o garfo abaixa de novo.
    public void DisableCollider()
    {
        if (hitbox != null)
            hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detecta jogador
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
             if (player != null && !player.invincible)
        {
            player.LoseLife();
        }
        }
    }
}
