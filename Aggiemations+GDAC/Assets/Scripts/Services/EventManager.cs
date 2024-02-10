using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    private ServiceContainerSO serviceContainer;

    private void Awake()
    {
        serviceContainer.EventManager = this;
    }


    // Player propogating events
    public Action<int, FighterCombatController.AttackInstance> OnPlayerHitByAttack;
}