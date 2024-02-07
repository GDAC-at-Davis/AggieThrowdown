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
    [field: TextArea]
    public string FighterDescription { get; private set; }

    [field: SerializeField]
    public bool IncludeInRoster { get; private set; } = true;

    [field: SerializeField]
    public MovementStats MoveStats { get; private set; }

    [field: Header("Dashing")]
    [field: SerializeField]
    public float DashVelocity { get; private set; }

    [field: SerializeField]
    public float DashAccelerationOverTime { get; private set; }

    [field: SerializeField]
    public float DashCooldown { get; private set; }

    [field: SerializeField]
    [field: Header("Hurtboxes")]
    public AttackConfig BasicAttack { get; private set; }

    [field: SerializeField]
    public AttackConfig HeavyAttack { get; private set; }

    [field: SerializeField]
    [field: Header("Visuals")]
    public AnimatorOverrideController AnimationOverrides { get; private set; }

    [field: SerializeField]

    public FighterManager FighterPrefab { get; private set; }
}

[Serializable]
public class AttackConfig
{
    [field: SerializeField]
    public List<Hurtbox> Hurtboxes { get; private set; }

    [field: SerializeField]
    public float Damage { get; private set; }

    [field: SerializeField]
    public float KnockbackVelocity { get; private set; }

    public void DrawHurtboxGizmos(Vector2 position, int xDir)
    {
        foreach (var hurtbox in Hurtboxes)
        {
            hurtbox.DrawHurtboxGizmo(position, xDir);
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
        var offset = new Vector2(Offset.x * xDir, Offset.y);
        var pos = position + offset;

        var size = Size;
        var angle = Angle;

        var halfSize = size / 2;

        var topLeft = pos + new Vector2(-halfSize.x, halfSize.y);
        var topRight = pos + new Vector2(halfSize.x, halfSize.y);
        var bottomLeft = pos + new Vector2(-halfSize.x, -halfSize.y);
        var bottomRight = pos + new Vector2(halfSize.x, -halfSize.y);

        var rot = Quaternion.Euler(0, 0, angle);

        topLeft = pos + (Vector2)(rot * (topLeft - pos));
        topRight = pos + (Vector2)(rot * (topRight - pos));
        bottomLeft = pos + (Vector2)(rot * (bottomLeft - pos));
        bottomRight = pos + (Vector2)(rot * (bottomRight - pos));

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
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