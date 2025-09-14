using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BreadSliceProjectile : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float lifetime = 5f;


    [Header("Colisão")]
    [Tooltip("Layers que destroem o pão ao colidir (ex.: terreno, obstáculos).")]
    public LayerMask destroyOnCollision;
    [Tooltip("Offset de rotação do sprite.")]
    public float rotationOffset = 0f;

    [Header("Efeitos (opcional)")]
    public GameObject hitEffectPrefab;

    [Header("Empurrão no Player")]
    public float pushForce = 8f;

    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.up;
    private bool launched = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // sempre controlado por script
    }

    private void Start()
    {
        if (!launched)
        {
            Vector2 dir = transform.up;
            Launch(dir, snapToAxis: true);
        }

        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 dir, bool snapToAxis)
    {
        launched = true;

        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.up;

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

        // Ajusta rotação do sprite
        float ang = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(ang + rotationOffset, Vector3.forward);
    }

    private void FixedUpdate()
    {
        // Movimento constante — nunca para
        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // --- jogador ---
        var player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            // aplica força de empurrão no jogador
            var prb = player.GetComponent<Rigidbody2D>();
            if (prb != null)
            {
                prb.AddForce(moveDir.normalized * pushForce, ForceMode2D.Impulse);
            }

            // pão continua indo sem parar (não se destrói no player)
            return;
        }

        // --- layers configuráveis ---
        if (((1 << collision.collider.gameObject.layer) & destroyOnCollision) != 0)
        {
            Destroy(gameObject);
            SpawnHitEffect();
        }
    }
    private void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
    }
}
