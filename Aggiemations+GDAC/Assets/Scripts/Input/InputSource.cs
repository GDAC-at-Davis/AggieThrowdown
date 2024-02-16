using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input sources are the drivers behind InputProviders, through the MultiInputManager
/// e.g Gamepad input source and Keyboard input source both drive input providers
/// </summary>
public abstract class InputSource
{
    protected MultiInputManager inputManager;

    protected List<InputProvider> inputProviders => inputManager.inputProviders;

    public InputSource(MultiInputManager inputManager)
    {
        this.inputManager = inputManager;
    }

    public abstract void Dispose();
}