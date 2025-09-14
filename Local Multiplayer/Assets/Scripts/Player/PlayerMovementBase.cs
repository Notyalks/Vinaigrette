using UnityEngine;
using System.Collections;

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

    [Header("Wall Movement")]
    public float wallSlideSpeed = 1.5f;
    public float wallJumpForce = 8f;
    public float wallJumpHorizontalForce = 5f;
    [Tooltip("Profundidade do box lateral de checagem da parede.")]
    [Range(0.05f, 0.5f)] public float wallProbeDepth = 0.15f;
    [Tooltip("Altura do box lateral de checagem da parede em relação ao collider.")]
    [Range(0.3f, 1.2f)] public float wallProbeHeightScale = 0.8f;
    public LayerMask wallLayer;

    [Header("Dash")]
    public float dashForce = 12f;          // força do dash
    public float dashDuration = 0.2f;      // tempo de duração
    public float dashCooldown = 0.5f;      // tempo antes de poder usar de novo
    public bool canAirDash = true;         // se pode usar no ar

    // Estado de movimento
    public Vector2 moveInput;
    protected bool jumpQueued;
    protected bool dashQueued;
    public bool isDashing = false;
    private bool dashOnCooldown = false;

    private bool grounded;
    public bool IsGrounded => grounded;
    public bool IsWallSliding => (!grounded && isTouchingWall && moveInput.x == wallDirection);
    private bool isTouchingWall;
    private int wallDirection;

    private bool inputBlocked = false;
    public bool InputBlocked => inputBlocked;
    // -1 = esquerda, +1 = direita

    // facing - guarda última direção horizontal não-nula
    // inicia 1 (direita)
    protected int facingDirection = 1;

    // buffers para evitar GC
    private static readonly Collider2D[] groundHits = new Collider2D[8];

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
        bodyCollider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        if (PauseMenu.GameIsPaused) return; // bloqueia input/movimento

        if (!controller.isActive || controller.hasFinished) return;

        grounded = CheckGrounded();
        CheckWall();

        HandleJump();
        HandleWallSlide();
        HandleDash();
    }

    protected virtual void FixedUpdate()
    {
        if (PauseMenu.GameIsPaused) return; //  bloqueia física do input

        if (!controller.isActive || controller.hasFinished) return;

        if (!horizontalLocked && !isDashing)
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
    // ----------------- Input -----------------
    // SetMove agora atualiza facingDirection quando apropriado
    public void SetMove(Vector2 input)
    {
        if (inputBlocked) return; // ignora
        moveInput = input;
        // atualiza direção apenas se houver input significativo e controle não estiver travado
        if (!horizontalLocked && Mathf.Abs(input.x) > 0.1f)
            facingDirection = input.x > 0f ? 1 : -1;
    }
    public void QueueJump()
    {
        if (!inputBlocked) // ignora
            jumpQueued = true;
    }


    public void QueueDash()
    {
        if (!inputBlocked) // ignora
            dashQueued = true;
    }

    public void BlockInputs(bool block)
    {
        inputBlocked = block;

        if (block)
        {
            // zera inputs ativos
            moveInput = Vector2.zero;
            jumpQueued = false;
            dashQueued = false;
        }
    }

    // ----------------- Jump -----------------
    private void HandleJump()
    {
        if (!jumpQueued || isDashing) // não pula durante dash
            return;

        if (grounded)
        {
            // Pulo normal
            if (rb.linearVelocity.y < 0f)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (isTouchingWall)
        {
            // Wall jump
            rb.linearVelocity = Vector2.zero;
            Vector2 force = new Vector2(-wallDirection * wallJumpHorizontalForce, wallJumpForce);
            rb.AddForce(force, ForceMode2D.Impulse);

            // trava o input horizontal por um tempo
            StartCoroutine(LockHorizontalControl(0.2f));
        }

        jumpQueued = false;
    }

    private bool horizontalLocked = false;

    private IEnumerator LockHorizontalControl(float duration)
    {
        horizontalLocked = true;
        yield return new WaitForSeconds(duration);
        horizontalLocked = false;
    }

    // ----------------- Wall Slide -----------------
    private void HandleWallSlide()
    {
        if (!grounded && isTouchingWall && Mathf.Sign(moveInput.x) == wallDirection)
        {
            // força a velocidade vertical para um valor constante de descida
            if (rb.linearVelocity.y < 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
            }
        }
    }

    // ----------------- Ground Check -----------------
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

    // ----------------- Wall Check -----------------
    private void CheckWall()
    {
        if (bodyCollider == null) return;

        Bounds b = bodyCollider.bounds;

        Vector2 size = new Vector2(wallProbeDepth, b.size.y * wallProbeHeightScale);
        Vector2 centerLeft = new Vector2(b.min.x - size.x * 0.5f, b.center.y);
        Vector2 centerRight = new Vector2(b.max.x + size.x * 0.5f, b.center.y);

        bool leftHit = Physics2D.OverlapBox(centerLeft, size, 0f, wallLayer) is Collider2D leftCol && !leftCol.isTrigger;
        bool rightHit = Physics2D.OverlapBox(centerRight, size, 0f, wallLayer) is Collider2D rightCol && !rightCol.isTrigger;


        if (leftHit)
        {
            isTouchingWall = true;
            wallDirection = -1;
        }
        else if (rightHit)
        {
            isTouchingWall = true;
            wallDirection = 1;
        }
        else
        {
            isTouchingWall = false;
            wallDirection = 0;
        }
    }

    // ----------------- Dash -----------------
    private void HandleDash()
    {
        if (dashQueued && !isDashing && !dashOnCooldown && (grounded || canAirDash))
        {
            Vector2 dashDir;

            // prioriza input.x; se não houver, usa facingDirection salvo
            if (Mathf.Abs(moveInput.x) > 0.1f)
                dashDir = new Vector2(Mathf.Sign(moveInput.x), 0f);
            else
                dashDir = new Vector2(facingDirection != 0 ? facingDirection : 1, 0f);

            StartCoroutine(DoDash(dashDir));
        }

        dashQueued = false;
    }

    private IEnumerator DoDash(Vector2 direction)
    {
        isDashing = true;
        dashOnCooldown = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero; // reset para consistência

        // garantir apenas horizontal
        direction.y = 0f;
        direction.Normalize();

        rb.AddForce(direction * dashForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }

    // ----------------- Gizmos -----------------
    private void OnDrawGizmosSelected()
    {
        var col = bodyCollider ?? GetComponent<Collider2D>();
        if (col == null) return;

        Bounds b = col.bounds;

        // Ground check
        Vector2 groundSize = new Vector2(b.size.x * groundProbeWidthScale, groundProbeDepth);
        Vector2 groundCenter = new Vector2(b.center.x, b.min.y - groundSize.y * 0.5f);
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(groundCenter, groundSize);

        // Wall check
        Vector2 wallSize = new Vector2(wallProbeDepth, b.size.y * wallProbeHeightScale);
        Vector2 centerLeft = new Vector2(b.min.x - wallSize.x * 0.5f, b.center.y);
        Vector2 centerRight = new Vector2(b.max.x + wallSize.x * 0.5f, b.center.y);

        Gizmos.color = isTouchingWall ? Color.green : Color.red;
        Gizmos.DrawWireCube(centerLeft, wallSize);
        Gizmos.DrawWireCube(centerRight, wallSize);
    }
}
