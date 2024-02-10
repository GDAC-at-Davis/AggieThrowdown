using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [field: SerializeField]
    public CinemachineVirtualCamera SpawningCamera { get; set; }

    [field: SerializeField]
    public bool FlipSprite { get; private set; }

    private void Awake()
    {
        SetCameraActive(false);
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public void SetCameraActive(bool value)
    {
        SpawningCamera.gameObject.SetActive(value);
    }
}