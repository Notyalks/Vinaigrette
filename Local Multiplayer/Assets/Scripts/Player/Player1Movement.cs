using UnityEngine;
using UnityEngine.InputSystem;

public class Player1Movement : PlayerMovementBase, PlayerInputActions.IPlayer1Actions
{
    private PlayerInputActions inputActions;

    protected override void Awake()
    {
        base.Awake();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player1.SetCallbacks(this);
        inputActions.Player1.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player1.Disable();
    }

    // Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        SetMove(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            QueueJump();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
            QueueDash();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
            Debug.Log("Pause Player1");
    }

}
