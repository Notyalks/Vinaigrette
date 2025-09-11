using UnityEngine;

public class SaltProjectile : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float lifetime = 5f;

    [Header("Dano")]
    public int damage = 1;

    [Header("Colisão")]
    [Tooltip("Layers que destroem o projétil ao colidir.")]
    public LayerMask destroyOnCollision;
    [Tooltip("Offset de rotação para alinhar sprite com a direção (ex.: 90 se sprite está deitado).")]
    public float rotationOffset = 0f;

    [Header("Efeitos (opcional)")]
    public GameObject hitEffectPrefab;

    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.right; // fallback
    private bool launched = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (!launched)
        {
            Vector2 dir = transform.right;
            Launch(dir, snapToAxis: true);
        }

        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 dir, bool snapToAxis)
    {
        launched = true;

        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.right;

        if (snapToAxis)
        {
            if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
                moveDir = new Vector2(Mathf.Sign(dir.x), 0f);
            else
                moveDir = new Vector2(0f, Mathf.Sign(dir.y));
        }
        else
        {
            moveDir = dir.normalized;
        }

        if (rb != null)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = moveDir * speed;
#else
            rb.velocity = moveDir * speed;
#endif
        }

        // Aplica rotação visual
        float ang = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(ang + rotationOffset, Vector3.forward);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // --- jogador ---
        var player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            // Só causa dano se estiver ativo e não invencível
            if (player.isActive && !player.invincible)
            {
                for (int i = 0; i < damage; i++)
                    player.LoseLife();
            }

            // Sempre destrói o projétil ao colidir com jogador, mesmo morto
            SpawnHitEffect();
            Destroy(gameObject);
            return;
        }

        // --- layers configuráveis ---
        if (((1 << collision.collider.gameObject.layer) & destroyOnCollision) != 0)
        {
            SpawnHitEffect();
            Destroy(gameObject);
        }
    }

    private void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
    }
}
