using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiInputManager : MonoBehaviour
{
    public static int MaxInputUsers = 2;

    [SerializeField]
    private ServiceContainerSO container;

    [SerializeField]
    private float inputBufferDuration;

    public readonly List<InputProvider> inputProviders = new();

    public readonly List<InputSource> inputSources = new();

    private void Awake()
    {
        container.InputManager = this;

        inputProviders.Add(new InputProvider());
        inputProviders.Add(new InputProvider());

        foreach (var inputProvider in inputProviders)
        {
            inputProvider.Initialize(inputBufferDuration);
        }

        inputSources.Add(new KeyboardInputSource(this));
    }

    private void OnDestroy()
    {
        foreach (var keyboardInputSource in inputSources)
        {
            keyboardInputSource.Dispose();
        }
    }

    private void Update()
    {
        foreach (var inputProvider in inputProviders)
        {
            inputProvider.Update();
        }
    }

    public InputProvider GetInputProvider(int playerIndex)
    {
        return inputProviders[playerIndex];
    }
}