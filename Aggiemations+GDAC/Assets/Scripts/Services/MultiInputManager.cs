using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiInputManager : MonoBehaviour, DefaultControls.IGameplayActions
{
    [SerializeField]
    private ServiceContainerSO container;

    [SerializeField]
    private float inputBufferDuration;

    private readonly List<InputProvider> inputProviders = new();
    private DefaultControls input;

    private void Awake()
    {
        container.InputManager = this;

        input = new DefaultControls();
        input.Enable();
        input.Gameplay.AddCallbacks(this);

        inputProviders.Add(new InputProvider());
        inputProviders.Add(new InputProvider());

        foreach (var inputProvider in inputProviders)
        {
            inputProvider.Initialize(inputBufferDuration);
        }
    }

    private void OnDestroy()
    {
        input.Disable();
    }

    private void Update()
    {
        foreach (var inputProvider in inputProviders)
        {
            inputProvider.Update();
        }
    }

    public void OnMovement_P1(InputAction.CallbackContext context)
    {
        inputProviders[0].MovementInput = context.ReadValue<Vector2>();
        inputProviders[0].TriggerOnMovementInput(context);
    }

    public void OnMovement_P2(InputAction.CallbackContext context)
    {
        inputProviders[1].MovementInput = context.ReadValue<Vector2>();
        inputProviders[1].TriggerOnMovementInput(context);
    }

    public void OnJump_P1(InputAction.CallbackContext context)
    {
        inputProviders[0].TriggerOnJumpInput(context);
    }

    public void OnJump_P2(InputAction.CallbackContext context)
    {
        inputProviders[1].TriggerOnJumpInput(context);
    }

    public void OnBasic_P1(InputAction.CallbackContext context)
    {
        inputProviders[0].TriggerOnBasicAttackInput(context);
    }

    public void OnBasic_P2(InputAction.CallbackContext context)
    {
        inputProviders[1].TriggerOnBasicAttackInput(context);
    }

    public void OnHeavy_P1(InputAction.CallbackContext context)
    {
        inputProviders[0].TriggerOnHeavyAttackInput(context);
    }

    public void OnHeavy_P2(InputAction.CallbackContext context)
    {
        inputProviders[1].TriggerOnHeavyAttackInput(context);
    }

    public void OnDash_P1(InputAction.CallbackContext context)
    {
        inputProviders[0].TriggerOnDashInput(context);
    }

    public void OnDash_P2(InputAction.CallbackContext context)
    {
        inputProviders[1].TriggerOnDashInput(context);
    }

    public void OnTaunt_P1(InputAction.CallbackContext context)
    {
        inputProviders[0].TriggerOnTauntInput(context);
    }

    public void OnTaunt_P2(InputAction.CallbackContext context)
    {
        inputProviders[1].TriggerOnTauntInput(context);
    }

    public InputProvider GetInputProvider(int playerIndex)
    {
        return inputProviders[playerIndex];
    }
}