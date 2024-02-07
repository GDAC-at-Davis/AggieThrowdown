using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterHitbox : MonoBehaviour
{
    public event Func<FighterCombatController.AttackInstance, bool> OnHitByAttack;

    public bool ReceiveAttack(FighterCombatController.AttackInstance attackInstance)
    {
        if (OnHitByAttack == null)
        {
            return false;
        }

        return OnHitByAttack.Invoke(attackInstance);
    }
}