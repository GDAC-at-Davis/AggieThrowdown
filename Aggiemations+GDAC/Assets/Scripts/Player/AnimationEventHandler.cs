using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    // Events
    public event Action<bool> OnSetSuperArmor;
    public event Action OnAttackActionImpact;
    public event Action OnFinishAction;

    public event Action<float> OnSetActionVelocityX;
    public event Action<float> OnSetActionVelocityY;

    public event Action<float> OnSetActionAccelerationX;
    public event Action<float> OnSetActionAccelerationY;
    public event Action<float> OnMultiplyCurrentVelocityX;
    public event Action<float> OnMultiplyCurrentVelocityY;

    public void StartSuperArmor()
    {
        OnSetSuperArmor?.Invoke(true);
    }

    public void EndSuperArmor()
    {
        OnSetSuperArmor?.Invoke(false);
    }

    public void TriggerAttackImpactHurtboxes()
    {
        OnAttackActionImpact?.Invoke();
    }

    public void FinishAction()
    {
        OnFinishAction?.Invoke();
    }

    public void SetActionVelocityX(float xVelocity)
    {
        OnSetActionVelocityX?.Invoke(xVelocity);
    }

    public void SetActionVelocityY(float yVelocity)
    {
        OnSetActionVelocityY?.Invoke(yVelocity);
    }

    public void SetActionAccelerationX(float xAcceleration)
    {
        OnSetActionAccelerationX?.Invoke(xAcceleration);
    }

    public void SetActionAccelerationY(float yAcceleration)
    {
        OnSetActionAccelerationY?.Invoke(yAcceleration);
    }

    public void MultiplyCurrentVelocityX(float xMultiplier)
    {
        OnMultiplyCurrentVelocityX?.Invoke(xMultiplier);
    }

    public void MultiplyCurrentVelocityY(float yMultiplier)
    {
        OnMultiplyCurrentVelocityY?.Invoke(yMultiplier);
    }
}