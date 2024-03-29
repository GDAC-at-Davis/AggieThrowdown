using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class MainCameraController : MonoBehaviour
{
    public static MainCameraController Instance { get; private set; }

    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    private CinemachineConfiner2D confiner;

    [SerializeField]
    private int confinerUpdateInterval;

    [FormerlySerializedAs("offset")]
    [Header("Aiming config")]
    [SerializeField]
    private Vector2 postOffset;

    [SerializeField]
    private float widthMargin;

    [SerializeField]
    private float heightMargin;

    [SerializeField]
    private float minOrthoWidth;

    [SerializeField]
    private float minOrthoHeight;

    [SerializeField]
    private float unitLerpFactor = 0.99f;

    [FormerlySerializedAs("universalImpulseSource")]
    [FormerlySerializedAs("impulseSource")]
    [Header("Impulse")]
    [SerializeField]
    private CinemachineImpulseSource globalImpulseSource;

    [SerializeField]
    private float impulseScale;

    private List<Transform> Targets = new();

    private void Awake()
    {
        Instance = this;
        CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
    }

    public void AddTarget(Transform target)
    {
        Targets.Add(target);
    }

    public void RemoveTarget(Transform target)
    {
        Targets.Remove(target);
    }

    private void Update()
    {
        if (Time.frameCount % confinerUpdateInterval == 0)
        {
            confiner.InvalidateCache();
        }
    }

    private void FixedUpdate()
    {
        // Calculate bounds
        var leftBound = float.MaxValue;
        var rightBound = float.MinValue;
        var lowerBound = float.MaxValue;
        var upperBound = float.MinValue;

        foreach (var target in Targets)
        {
            if (target == null)
            {
                continue;
            }

            var position = target.position;
            position.z = 0;
            if (position.x < leftBound)
            {
                leftBound = position.x;
            }

            if (position.x > rightBound)
            {
                rightBound = position.x;
            }

            if (position.y > upperBound)
            {
                upperBound = position.y;
            }

            if (position.y < lowerBound)
            {
                lowerBound = position.y;
            }
        }

        // Add margins
        leftBound -= widthMargin;
        rightBound += widthMargin;
        lowerBound -= heightMargin;
        upperBound += heightMargin;

        var width = Mathf.Max(minOrthoWidth, rightBound - leftBound);
        var height = Mathf.Max(minOrthoHeight, upperBound - lowerBound);

        // Decide on width driven or height driven
        var aspectRatio = (float)Screen.currentResolution.height / Screen.currentResolution.width;

        var heightFromWidth = width * aspectRatio;
        var newOrthoSize = Mathf.Max(heightFromWidth, height);

        // Apply resize to virtual camera with smoothing
        var currentOrthoSize = virtualCamera.m_Lens.OrthographicSize;

        var t = 1 - Mathf.Pow(1 - unitLerpFactor, Time.deltaTime);
        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentOrthoSize, newOrthoSize / 2, t);

        // Reposition
        Vector2 currentPos = virtualCamera.transform.position;
        var centerPos = new Vector2((leftBound + rightBound) / 2, (lowerBound + upperBound) / 2);
        Vector3 cameraPos = Vector2.Lerp(currentPos, centerPos, t);
        cameraPos.z = -10;
        virtualCamera.transform.position = cameraPos + (Vector3)postOffset;
    }

    public void SendImpulse(Vector2 velocity)
    {
        globalImpulseSource.GenerateImpulse(velocity * impulseScale);
    }
}