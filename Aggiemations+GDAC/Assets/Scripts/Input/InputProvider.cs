using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider
{
    public Vector2 MovementInput { get; set; }

    public event Action<InputAction.CallbackContext> OnJumpInput;
    public event Action<InputAction.CallbackContext> OnMovementInput;
    public event Action<InputAction.CallbackContext> OnBasicAttackInput;
    public event Action<InputAction.CallbackContext> OnHeavyAttackInput;
    public event Action<InputAction.CallbackContext> OnDashInput;
    public event Action<InputAction.CallbackContext> OnTauntInput;


    public void TriggerOnJumpInput(InputAction.CallbackContext context)
    {
        OnJumpInput?.Invoke(context);
    }

    public void TriggerOnMovementInput(InputAction.CallbackContext context)
    {
        OnMovementInput?.Invoke(context);
    }

    public void TriggerOnBasicAttackInput(InputAction.CallbackContext context)
    {
        OnBasicAttackInput?.Invoke(context);
    }

    public void TriggerOnHeavyAttackInput(InputAction.CallbackContext context)
    {
        OnHeavyAttackInput?.Invoke(context);
    }

    public void TriggerOnDashInput(InputAction.CallbackContext context)
    {
        OnDashInput?.Invoke(context);
    }

    public void TriggerOnTauntInput(InputAction.CallbackContext context)
    {
        OnTauntInput?.Invoke(context);
    }
}