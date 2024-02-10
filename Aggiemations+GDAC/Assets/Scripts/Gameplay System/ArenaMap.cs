using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ArenaMap : MonoBehaviour
{
    [field: SerializeField]
    public List<SpawnPoint> SpawnPoints { get; private set; }

    [field: SerializeField]
    public CinemachineVirtualCamera ArenaOverviewCamera { get; private set; }

    private void Awake()
    {
        SetArenaCameraActive(false);
    }

    public void SetArenaCameraActive(bool value)
    {
        ArenaOverviewCamera.gameObject.SetActive(value);
    }
}