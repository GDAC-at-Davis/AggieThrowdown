using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class GamepadInputBinder : MonoBehaviour
{
    [SerializeField]
    private MultiInputManager multiInputManager;

    private int playerCount;

    private void Start()
    {
        InputUser.listenForUnpairedDeviceActivity = MultiInputManager.MaxInputUsers;

        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
    }

    private void OnDestroy()
    {
        InputUser.listenForUnpairedDeviceActivity = 0;
        InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
    }

    private void OnUnpairedDeviceUsed(InputControl control, InputEventPtr ptr)
    {
        // Debug.Log($"Unpaired device used: {control.device.description}");
        var device = control.device;

        var isValidDevice = device is Gamepad;
        if (!isValidDevice)
        {
            return;
        }

        SetupExistingProvider(device);
    }

    private void SetupExistingProvider(InputDevice device)
    {
        multiInputManager.inputSources.Add(new GamepadInputSource(
            multiInputManager,
            playerCount,
            new[] { device }
        ));

        if (InputUser.listenForUnpairedDeviceActivity > 0)
        {
            InputUser.listenForUnpairedDeviceActivity--;
            playerCount++;
        }
    }
}