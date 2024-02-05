using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FighterConfig", menuName = "Fighter Config")]
public class FighterConfigSO : ScriptableObject
{
    [field: SerializeField]
    [field: TextArea]
    public string FighterName { get; private set; }

    [field: SerializeField]
    public MovementStats MoveStats { get; private set; }
}

[Serializable]
public class MovementStats
{
    [Header("Stats")]
    public float GroundMaxMovementSpeed;

    public float MovementAcceleration;

    [Header("Air")]
    public float JumpVelocity;

    public float AirMovementAcceleration;
    public float AirMaxMovementSpeed;
    public float FastFallAcceleration;

    [Header("Grounding")]
    public float MaxGroundStableAngle;

    public float GroundCastRadius;
    public float GroundCastDistance;
    public float GroundCastOffset;

    public LayerMask groundMask;
}