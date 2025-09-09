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
        if (!controller.isActive) return;

        // Idle/Walk
        animator.SetFloat("Speed", Mathf.Abs(movement.moveInput.x));

        // Jump
        animator.SetBool("IsGrounded", movement.IsGrounded);

        // Dash
        animator.SetBool("IsDashing", movement.isDashing);

        // Wall Slide
        animator.SetBool("IsWallSliding", movement.IsWallSliding);

        // Death
        animator.SetBool("IsDead", !controller.isActive || controller.lives <= 0);

        // -------- FlipX --------
        if (movement.moveInput.x > 0.01f)
            spriteRenderer.flipX = false; // olhando para a direita
        else if (movement.moveInput.x < -0.01f)
            spriteRenderer.flipX = true; // olhando para a esquerda
    }
}
