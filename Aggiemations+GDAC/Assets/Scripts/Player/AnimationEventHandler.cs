using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    // Events
    public event Action OnBeginDash;
    public event Action<bool> OnSetInvincible;
    public event Action<bool> OnSetSuperArmor;
    public event Action OnHeavyAttackImpact;
    public event Action OnLightAttackImpact;
    public event Action OnFinishAction;

    public void BeginDash()
    {
        OnBeginDash?.Invoke();
    }

    public void StartInvincibleFrames()
    {
        OnSetInvincible?.Invoke(true);
    }

    public void EndInvincibleFrames()
    {
        OnSetInvincible?.Invoke(false);
    }

    public void StartSuperArmor()
    {
        OnSetSuperArmor?.Invoke(true);
    }

    public void EndSuperArmor()
    {
        OnSetSuperArmor?.Invoke(false);
    }

    public void HeavyAttackImpact()
    {
        OnHeavyAttackImpact?.Invoke();
    }

    public void LightAttackImpact()
    {
        OnLightAttackImpact?.Invoke();
    }

    public void FinishAction()
    {
        OnFinishAction?.Invoke();
    }

    public void Test(float val)
    {
    }
}