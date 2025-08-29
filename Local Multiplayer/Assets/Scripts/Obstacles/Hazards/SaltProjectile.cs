using UnityEngine;

public class SaltProjectile : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float lifetime = 5f;

    [Header("Dano")]
    public int damage = 1;

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
        // Caso o atirador não tenha chamado Launch, tenta inferir pela rotação atual
        if (!launched)
        {
            Vector2 dir = transform.right; // direção local "frente" do projétil
            Launch(dir, snapToAxis: true);
        }

        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Lança o projétil em uma direção. Se snapToAxis = true, trava no eixo dominante (X ou Y),
    /// respeitando o sinal (+X, -X, +Y, -Y).
    /// </summary>
    public void Launch(Vector2 dir, bool snapToAxis)
    {
        launched = true;

        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.right; // fallback seguro

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

        // Opcional: alinha a rotação visual com a direção de movimento
        float ang = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.collider.GetComponent<PlayerController>();
        if (player != null && player.isActive && !player.invincible)
        {
            for (int i = 0; i < damage; i++)
                player.LoseLife();

            SpawnHitEffect();
            Destroy(gameObject);
            return;
        }

        if (collision.collider.CompareTag("Obstacles") ||
            collision.collider.CompareTag("Terrain") ||
            collision.collider.CompareTag("Ground"))
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
