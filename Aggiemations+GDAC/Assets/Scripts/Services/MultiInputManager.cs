using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiInputManager : MonoBehaviour, DefaultControls.IGameplayActions
{
    [SerializeField]
    private ServiceContainerSO container;

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
    }

    private void OnDestroy()
    {
        input.Disable();
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

    public InputProvider GetInputProvider(int playerIndex)
    {
        return inputProviders[playerIndex];
    }
}