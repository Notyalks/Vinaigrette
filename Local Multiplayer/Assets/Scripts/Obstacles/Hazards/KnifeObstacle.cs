using UnityEngine;

public class KnifeObstacle : ActiveObstacleBase
{
    [Header("Configurações de Rotação")]
    public float rotationSpeed = 60f; // Velocidade da rotação em graus por segundo
    public bool clockwise = true;     // Se a rotação será no sentido horário ou anti-horário

    [Header("Som de impacto")]
    public AudioClip hitSFX;

    private SpriteRenderer sr;

    protected override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
    }

    protected override void ActiveUpdate()
    {
        float direction = clockwise ? -1f : 1f;

        // centro do sprite em coordenadas de mundo
        Vector3 pivotWorld = sr.bounds.center;

        // gira em torno do centro real do sprite
        transform.RotateAround(pivotWorld, Vector3.forward, rotationSpeed * direction * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null && !player.invincible)
        {
            // toca som de impacto
            if (hitSFX != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(hitSFX);
            }

            player.LoseLife();
        }
    }
}
