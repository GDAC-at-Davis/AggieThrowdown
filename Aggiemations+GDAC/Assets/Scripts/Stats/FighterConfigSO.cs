using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FighterConfig", menuName = "Fighter Config")]
public class FighterConfigSO : ScriptableObject
{
    [field: SerializeField]
    [field: TextArea]
    public string FighterName { get; set; }

    [field: SerializeField]
    public Sprite FighterPortrait { get; set; }

    [field: SerializeField]
    [field: TextArea]
    public string FighterDescription { get; set; }

    [field: SerializeField]
    public bool IncludeInRoster { get; set; }

    [field: SerializeField]
    public bool FastLoadForTest { get; set; }

    [field: SerializeField]
    public MovementStats MoveStats { get; private set; }

    [field: Header("Dashing")]
    [field: SerializeField]
    public float DashCooldown { get; private set; }

    [field: SerializeField]
    [field: Header("Hurtboxes")]
    public AttackConfig BasicAttack { get; private set; }

    [field: SerializeField]
    public AttackConfig HeavyAttack { get; private set; }

    [field: SerializeField]
    [field: Header("Dependencies (Do not modify directly)")]
    public AnimatorOverrideController AnimationOverrides { get; set; }

    [field: SerializeField]

    public FighterManager FighterPrefab { get; set; }

    [field: SerializeField]
    public AudioConfig AudioConfig { get; private set; }
}

[Serializable]
public class AttackConfig
{
    [field: SerializeField]
    public List<Hurtbox> Hurtboxes { get; private set; }

    [field: SerializeField]
    public int PointsAwarded { get; private set; }

    [field: SerializeField]
    public Vector2 KnockbackVelocity { get; private set; }

    public void DrawHurtboxGizmos(Vector2 position, int xDir)
    {
        foreach (var hurtbox in Hurtboxes)
        {
            hurtbox.DrawHurtboxGizmo(position, xDir);
        }
    }

    public void DrawHurtboxDebug(Vector2 position, int xDir, float duration)
    {
        foreach (var hurtbox in Hurtboxes)
        {
            hurtbox.DrawHurtboxDebug(position, xDir, duration);
        }
    }
}

[Serializable]
public class Hurtbox
{
    [field: SerializeField]
    public Vector2 Offset { get; private set; }

    [field: SerializeField]
    public Vector2 Size { get; private set; }

    [field: SerializeField]
    public float Angle { get; private set; }

    public void DrawHurtboxGizmo(Vector2 position, int xDir)
    {
        CalculateCorners(position, xDir, out var topLeft, out var topRight, out var bottomLeft, out var bottomRight);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    public void DrawHurtboxDebug(Vector2 position, int xDir, float duration)
    {
        CalculateCorners(position, xDir, out var topLeft, out var topRight, out var bottomLeft, out var bottomRight);
        Debug.DrawLine(topLeft, topRight, Color.red, duration);
        Debug.DrawLine(topRight, bottomRight, Color.red, duration);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red, duration);
        Debug.DrawLine(bottomLeft, topLeft, Color.red, duration);
    }

    private void CalculateCorners(Vector2 position, int xDir, out Vector2 topLeft, out Vector2 topRight,
        out Vector2 bottomLeft, out Vector2 bottomRight)
    {
        var offset = new Vector2(Offset.x * xDir, Offset.y);
        var pos = position + offset;

        var size = Size;
        var angle = Angle;

        var halfSize = size / 2;

        topLeft = pos + new Vector2(-halfSize.x, halfSize.y);
        topRight = pos + new Vector2(halfSize.x, halfSize.y);
        bottomLeft = pos + new Vector2(-halfSize.x, -halfSize.y);
        bottomRight = pos + new Vector2(halfSize.x, -halfSize.y);

        var rot = Quaternion.Euler(0, 0, xDir * angle);

        topLeft = pos + (Vector2)(rot * (topLeft - pos));
        topRight = pos + (Vector2)(rot * (topRight - pos));
        bottomLeft = pos + (Vector2)(rot * (bottomLeft - pos));
        bottomRight = pos + (Vector2)(rot * (bottomRight - pos));
    }
}

[Serializable]
public class MovementStats
{
    [field: Header("Stats")]
    [field: SerializeField]
    public float GroundMaxMovementSpeed { get; private set; }

    [field: SerializeField]
    public float MovementAcceleration { get; private set; }

    [field: Header("Air")]
    [field: SerializeField]
    public float JumpVelocity { get; private set; }

    [field: SerializeField]
    public float AirMovementAcceleration { get; private set; }

    [field: SerializeField]
    public float AirMaxMovementSpeed { get; private set; }

    [field: SerializeField]
    public float Gravity { get; private set; }

    [field: SerializeField]
    public float FastFallVelocity { get; private set; }

    [field: Header("Grounding")]
    [field: SerializeField]
    public float MaxGroundStableAngle { get; private set; }

    [field: SerializeField]
    public float GroundCastRadius { get; private set; }

    [field: SerializeField]
    public float GroundCastDistance { get; private set; }

    [field: SerializeField]
    public float GroundCastOffset { get; private set; }

    public LayerMask groundMask;
}

[Serializable]
public class AudioConfig
{
    [field: SerializeField]
    public AudioClip JumpClip { get; private set; }

    [field: SerializeField]
    public AudioClip DashClip { get; private set; }

    [field: SerializeField]
    public AudioClip BasicAttackClip { get; private set; }

    [field: SerializeField]
    public AudioClip BasicAttackImpactClip { get; private set; }

    [field: SerializeField]
    public AudioClip HeavyAttackClip { get; private set; }

    [field: SerializeField]
    public AudioClip HeavyAttackImpactClip { get; private set; }

    [field: SerializeField]
    public AudioClip TauntClip { get; private set; }
}