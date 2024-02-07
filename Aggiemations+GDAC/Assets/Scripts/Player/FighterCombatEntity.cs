using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterCombatEntity : MonoBehaviour
{
    private bool IsInvincible = false;
    private bool IsSuperArmor = false;

    public void SetInvincible(bool value)
    {
        IsInvincible = value;
    }

    public void SetSuperArmor(bool value)
    {
        IsSuperArmor = value;
    }

    public void ReceiveAttack()
    {
    }
}