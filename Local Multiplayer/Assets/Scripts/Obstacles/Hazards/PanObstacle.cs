using UnityEngine;

public class PanObstacle : ActiveObstacleBase
{
    [Header("Configurações do Impulso")]
    public float bounceForce = 12f;   // Intensidade do impulso
    public bool resetVelocity = true; // Se deve zerar a velocidade antes de aplicar o impulso

    private Collider2D col;

    protected override void Start()
    {
        base.Start();
        col = GetComponent<Collider2D>();
        if (col == null)
            Debug.LogWarning("PanObstacle precisa de um Collider2D!");
    }

    protected override void ActiveUpdate()
    {
        // A frigideira não tem movimento, só reage a colisões
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive) return;

        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player != null && player.isActive)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // A direção do impulso é a "frente" da frigideira (seu eixo local Y)
                Vector2 launchDirection = transform.up.normalized;

                if (resetVelocity)
                    rb.linearVelocity = Vector2.zero;

                rb.AddForce(launchDirection * bounceForce, ForceMode2D.Impulse);
            }
        }
    }
}
