using System;
using System.Collections.Generic;
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
        public InputAction.CallbackContext context;
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
            context = context
        });
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
        OnJumpInput?.Invoke(context);
        QueueInputToBuffer(InputType.Jump, context);
    }

    public void TriggerOnMovementInput(InputAction.CallbackContext context)
    {
        OnMovementInput?.Invoke(context);
    }

    public void TriggerOnBasicAttackInput(InputAction.CallbackContext context)
    {
        OnBasicAttackInput?.Invoke(context);
        QueueInputToBuffer(InputType.BasicAttack, context);
    }

    public void TriggerOnHeavyAttackInput(InputAction.CallbackContext context)
    {
        OnHeavyAttackInput?.Invoke(context);
        QueueInputToBuffer(InputType.HeavyAttack, context);
    }

    public void TriggerOnDashInput(InputAction.CallbackContext context)
    {
        OnDashInput?.Invoke(context);
        QueueInputToBuffer(InputType.Dash, context);
    }

    public void TriggerOnTauntInput(InputAction.CallbackContext context)
    {
        OnTauntInput?.Invoke(context);
    }
}