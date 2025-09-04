using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class PlayerMovementBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected PlayerController controller;
    protected Collider2D bodyCollider;

    [Header("Movimento")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Grounding")]
    [Tooltip("Layers consideradas chão/plataformas (ex.: Ground, Obstacles).")]
    public LayerMask groundLayer;
    [Tooltip("Profundidade do box de checagem sob os pés.")]
    [Range(0.01f, 0.3f)] public float groundProbeDepth = 0.15f;
    [Tooltip("Largura do box de checagem em relação à largura do collider.")]
    [Range(0.1f, 1.2f)] public float groundProbeWidthScale = 0.9f;

    protected Vector2 moveInput;
    protected bool jumpQueued;
    protected bool dashQueued;

    private bool grounded;
    public bool IsGrounded => grounded;

    private static readonly Collider2D[] groundHits = new Collider2D[8];

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
        bodyCollider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        if (!controller.isActive || controller.hasFinished) return;

        grounded = CheckGrounded();

        if (jumpQueued)
        {
            if (grounded)
            {
                // Zera velocidade negativa para pulo mais responsivo
                if (rb.velocity.y < 0f)
                    rb.velocity = new Vector2(rb.velocity.x, 0f);

                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            jumpQueued = false;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!controller.isActive || controller.hasFinished) return;

        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }

    public void SetMove(Vector2 input) => moveInput = input;
    public void QueueJump() => jumpQueued = true;
    public void QueueDash() => dashQueued = true;

    private bool CheckGrounded()
    {
        if (bodyCollider == null) return false;

        Bounds b = bodyCollider.bounds;

        Vector2 size = new Vector2(b.size.x * groundProbeWidthScale, groundProbeDepth);
        Vector2 center = new Vector2(b.center.x, b.min.y - size.y * 0.5f);

        int count = Physics2D.OverlapBoxNonAlloc(center, size, 0f, groundHits, groundLayer);

        for (int i = 0; i < count; i++)
        {
            var hit = groundHits[i];
            if (hit == null) continue;

            if (!hit.isTrigger && hit.attachedRigidbody != rb)
                return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        var col = bodyCollider ?? GetComponent<Collider2D>();
        if (col == null) return;

        Bounds b = col.bounds;
        Vector2 size = new Vector2(b.size.x * groundProbeWidthScale, groundProbeDepth);
        Vector2 center = new Vector2(b.center.x, b.min.y - size.y * 0.5f);

        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
