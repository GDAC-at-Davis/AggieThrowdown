using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Input provider represents the interface between the input system and users 
/// </summary>
public class InputProvider
{
    public Vector2 MovementInput { get; set; }

    public event Action<InputContext> OnJumpInput;
    public event Action<InputContext> OnMovementInput;
    public event Action<InputContext> OnBasicAttackInput;
    public event Action<InputContext> OnHeavyAttackInput;
    public event Action<InputContext> OnDashInput;
    public event Action<InputContext> OnTauntInput;

    private float bufferDuration;

    private enum InputType
    {
        Jump,
        BasicAttack,
        HeavyAttack,
        Dash
    }

    private struct BufferEntry
    {
        public InputType inputType;
        public float time;
        public InputContext context;
    }

    public struct InputContext
    {
        public bool buffered;
        public InputActionPhase phase;
        public InputAction.CallbackContext callbackContext;
    }

    private List<BufferEntry> inputBuffer = new();

    public void Initialize(float bufferDuration)
    {
        this.bufferDuration = bufferDuration;
    }

    private void QueueInputToBuffer(InputType type, InputAction.CallbackContext context)
    {
        inputBuffer.Add(new BufferEntry
        {
            inputType = type,
            time = Time.time,
            context = ContextFromCallback(context, true)
        });
    }

    private InputContext ContextFromCallback(InputAction.CallbackContext context, bool buffered = false)
    {
        return new InputContext
        {
            phase = context.phase,
            callbackContext = context,
            buffered = buffered
        };
    }

    public void Update()
    {
        for (var i = 0; i < inputBuffer.Count; i++)
        {
            var entry = inputBuffer[i];
            if (Time.time - entry.time < bufferDuration)
            {
                switch (entry.inputType)
                {
                    case InputType.Jump:
                        OnJumpInput?.Invoke(entry.context);
                        break;
                    case InputType.BasicAttack:
                        OnBasicAttackInput?.Invoke(entry.context);
                        break;
                    case InputType.HeavyAttack:
                        OnHeavyAttackInput?.Invoke(entry.context);
                        break;
                    case InputType.Dash:
                        OnDashInput?.Invoke(entry.context);
                        break;
                }
            }
            else
            {
                inputBuffer.RemoveAt(i);
                i--;
            }
        }
    }

    public void TriggerOnJumpInput(InputAction.CallbackContext context)
    {
        OnJumpInput?.Invoke(ContextFromCallback(context));
        QueueInputToBuffer(InputType.Jump, context);
    }

    public void TriggerOnMovementInput(InputAction.CallbackContext context)
    {
        OnMovementInput?.Invoke(ContextFromCallback(context));
    }

    public void TriggerOnBasicAttackInput(InputAction.CallbackContext context)
    {
        OnBasicAttackInput?.Invoke(ContextFromCallback(context));
        QueueInputToBuffer(InputType.BasicAttack, context);
    }

    public void TriggerOnHeavyAttackInput(InputAction.CallbackContext context)
    {
        OnHeavyAttackInput?.Invoke(ContextFromCallback(context));
        QueueInputToBuffer(InputType.HeavyAttack, context);
    }

    public void TriggerOnDashInput(InputAction.CallbackContext context)
    {
        OnDashInput?.Invoke(ContextFromCallback(context));
        QueueInputToBuffer(InputType.Dash, context);
    }

    public void TriggerOnTauntInput(InputAction.CallbackContext context)
    {
        OnTauntInput?.Invoke(ContextFromCallback(context));
    }
}