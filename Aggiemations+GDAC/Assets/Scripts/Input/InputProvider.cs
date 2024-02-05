using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider
{
    public Vector2 MovementInput { get; set; }

    public event Action<InputAction.CallbackContext> OnJumpInput;
    public event Action<InputAction.CallbackContext> OnMovementInput;

    public void TriggerOnJumpInput(InputAction.CallbackContext context)
    {
        OnJumpInput?.Invoke(context);
    }

    public void TriggerOnMovementInput(InputAction.CallbackContext context)
    {
        OnMovementInput?.Invoke(context);
    }
}