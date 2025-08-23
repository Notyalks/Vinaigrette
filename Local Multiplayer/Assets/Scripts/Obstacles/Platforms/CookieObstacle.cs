using UnityEngine;

public class CookieObstacle : ActiveObstacleBase
{
    [Header("Configurações da Bolacha")]
    public int maxSteps = 2;             // Quantas vezes pode ser pisada
    public float breakDelay = 0.5f;      // Tempo antes de sumir após quebrar
    public GameObject brokenEffectPrefab; // Efeito de quebrar (opcional)

    private int currentSteps = 0;        // Quantas vezes já foi pisada
    private bool isBroken = false;       // Estado atual
    private Collider2D col;              // Collider da plataforma
    private SpriteRenderer sr;           // Renderer (pra ocultar ao quebrar)

    protected override void Start()
    {
        base.Start();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive || isBroken) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            currentSteps++;

            if (currentSteps >= maxSteps)
            {
                Break();
            }
        }
    }

    private void Break()
    {
        isBroken = true;

        if (brokenEffectPrefab != null)
        {
            Instantiate(brokenEffectPrefab, transform.position, Quaternion.identity);
        }

        // Desativa o visual e colisão após um pequeno delay
        Invoke(nameof(HidePlatform), breakDelay);
    }

    private void HidePlatform()
    {
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;
    }

    protected override void ResetObstacle()
    {
        base.ResetObstacle();

        currentSteps = 0;
        isBroken = false;

        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;
    }
    protected override void ActiveUpdate() { }
}
