using UnityEngine;
using UnityEngine.InputSystem;

public class Player2Movement : PlayerMovementBase, PlayerInputActions.IPlayer2Actions
{
    private PlayerInputActions inputActions;

    protected override void Awake()
    {
        base.Awake();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player2.SetCallbacks(this);
        inputActions.Player2.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player2.Disable();
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
            Debug.Log("Pause Player2");
    }
}
