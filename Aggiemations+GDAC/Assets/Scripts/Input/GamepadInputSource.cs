using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class GamepadInputSource : InputSource, GamepadControls.IGameplayActions
{
    public int PlayerIndex { get; private set; }

    protected InputUser user;
    protected GamepadControls inputMap;

    public override void Dispose()
    {
        user.UnpairDevicesAndRemoveUser();
    }

    public GamepadInputSource(MultiInputManager inputManager, int playerIndex, InputDevice[] boundDevices) :
        base(inputManager)
    {
        // New instance of the action map
        PlayerIndex = playerIndex;
        inputMap = new GamepadControls();
        inputMap.Enable();

        // Pair bound devices to the user
        if (!user.valid)
        {
            user = InputUser.PerformPairingWithDevice(boundDevices[0]);
        }
        else
        {
            InputUser.PerformPairingWithDevice(boundDevices[0], user);
        }

        for (var i = 1; i < boundDevices.Length; i++)
        {
            InputUser.PerformPairingWithDevice(boundDevices[i], user);
        }

        // Pair action map with user
        user.AssociateActionsWithUser(inputMap);

        var scheme = InputControlScheme.FindControlSchemeForDevice(user.pairedDevices[0], user.actions.controlSchemes);
        if (scheme != null)
        {
            user.ActivateControlScheme(scheme.Value);
        }

        // Add callbacks
        inputMap.Gameplay.SetCallbacks(this);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        inputProviders[PlayerIndex].MovementInput = context.ReadValue<Vector2>();
        inputProviders[PlayerIndex].TriggerOnMovementInput(context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        inputProviders[PlayerIndex].TriggerOnJumpInput(context);
    }

    public void OnBasic(InputAction.CallbackContext context)
    {
        inputProviders[PlayerIndex].TriggerOnBasicAttackInput(context);
    }

    public void OnHeavy(InputAction.CallbackContext context)
    {
        inputProviders[PlayerIndex].TriggerOnHeavyAttackInput(context);
    }

    public void OnDash_Left(InputAction.CallbackContext context)
    {
        inputProviders[PlayerIndex].TriggerOnDashInput(context, -1);
    }

    public void OnDash_Right(InputAction.CallbackContext context)
    {
        inputProviders[PlayerIndex].TriggerOnDashInput(context, 1);
    }

    public void OnTaunt(InputAction.CallbackContext context)
    {
        inputProviders[PlayerIndex].TriggerOnTauntInput(context);
    }
}