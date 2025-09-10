using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerMovementBase movement;
    private PlayerController controller;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovementBase>();
        controller = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (animator == null || controller == null || movement == null) return;

        // ====== Outros parâmetros (apenas se NÃO estiver morto) ======
        animator.SetFloat("Speed", Mathf.Abs(movement.moveInput.x));
        animator.SetBool("IsGrounded", movement.IsGrounded);
        animator.SetBool("IsDashing", movement.isDashing);
        animator.SetBool("IsWallSliding", movement.IsWallSliding);

        // -------- Flip do sprite de acordo com input (mantém facing) --------
        if (movement.moveInput.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (movement.moveInput.x < -0.01f)
            spriteRenderer.flipX = true;
    }
}
