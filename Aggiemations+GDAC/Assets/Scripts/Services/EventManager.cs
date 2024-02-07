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

    public delegate void PlayerEventVoid(int playerIndex);

    // Player propogating events
    public PlayerEventVoid OnPlayerDeath;
}