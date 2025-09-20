using UnityEngine;

public class SaltProjectile : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float lifetime = 5f;

    [Header("Dano")]
    public int damage = 1;

    [Header("Colisão")]
    [Tooltip("Layers que destroem o projétil ao colidir (Collision).")]
    public LayerMask destroyOnCollision;

    [Tooltip("Layers que destroem o projétil ao entrar no Trigger.")]
    public LayerMask destroyOnTrigger;

    [Tooltip("Offset de rotação para alinhar sprite com a direção (ex.: 90 se sprite está deitado).")]
    public float rotationOffset = 0f;

    [Header("Efeitos (opcional)")]
    public GameObject hitEffectPrefab;
    public float hitEffectLifetime = 2f;

    [Header("Som de destruição (opcional)")]
    public AudioClip destroySFX;

    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.right;
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

        float ang = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(ang + rotationOffset, Vector3.forward);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            if (player.isActive && !player.invincible)
            {
                for (int i = 0; i < damage; i++)
                    player.LoseLife();
            }

            SpawnHitEffect();
            Destroy(gameObject);
            return;
        }

        if (((1 << collision.collider.gameObject.layer) & destroyOnCollision) != 0)
        {
            SpawnHitEffect();
            PlayDestroySFX();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & destroyOnTrigger) != 0)
        {
            SpawnHitEffect();
            PlayDestroySFX();
            Destroy(gameObject);
        }
    }

    private void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, hitEffectLifetime);
        }
    }

    private void PlayDestroySFX()
    {
        if (destroySFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(destroySFX);
        }
    }
}
