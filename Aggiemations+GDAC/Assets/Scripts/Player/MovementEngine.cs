using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public struct MovementDependencies
{
    public Rigidbody2D Rb;
    public Collider2D physicsCollider;
}

[Serializable]
public struct MovementContext
{
    public bool IsGrounded;
    public bool IsStableOnGround;

    public Vector2 GroundNormalSmoothed;

    public Vector2 GroundNormal;

    public float LastGroundedTime;
}

public class MovementEngine : MonoBehaviour
{
    public MovementContext Context;

    private float forceUngroundedTimer;

    public void Move(float horizontalInput, MovementStats stats, MovementDependencies dependencies)
    {
        // Timers
        forceUngroundedTimer -= Time.fixedDeltaTime;

        // Test grounded
        var newContext = new MovementContext();
        newContext.LastGroundedTime = Context.LastGroundedTime;

        CalculateGrounded(stats, dependencies, ref newContext);

        // Gravity
        if (!newContext.IsStableOnGround)
        {
            dependencies.Rb.velocity += Vector2.down * (stats.Gravity * Time.fixedDeltaTime);
        }
        else
        {
            newContext.LastGroundedTime = Time.time;
        }

        GroundSnap(horizontalInput, stats, dependencies, ref newContext);

        DoMovement(horizontalInput, stats, dependencies, ref newContext);

        Context = newContext;
    }

    public void ForceUnground(float time)
    {
        forceUngroundedTimer = time;
    }

    private void GroundSnap(float input, MovementStats stats, MovementDependencies dependencies,
        ref MovementContext newContext)
    {
        if (newContext.IsStableOnGround)
        {
            var vel = dependencies.Rb.velocity;
            var movingAwayFromGround = Vector2.Dot(vel, newContext.GroundNormal);
            var moveTowardsSlope = Vector2.Dot(Vector2.right * input, -newContext.GroundNormal);

            // Use smooth if moving towards the slope, raw if moving off the slope
            var snapNormal = moveTowardsSlope >= 0 ? newContext.GroundNormal : newContext.GroundNormalSmoothed;

            // Moving away from normal
            if (movingAwayFromGround >= 0)
            {
                vel = vel.ProjectOntoPlane(snapNormal);
            }

            dependencies.Rb.velocity = vel;
        }
    }

    private void DoMovement(float horizontalInput, MovementStats stats, MovementDependencies dependencies,
        ref MovementContext newContext)
    {
        var targetDir = new Vector2(horizontalInput, 0).ProjectOntoPlane(newContext.GroundNormal);
        var targetVelocity = targetDir *
                             (newContext.IsStableOnGround ? stats.GroundMaxMovementSpeed : stats.AirMaxMovementSpeed);

        var currentVel = dependencies.Rb.velocity.ProjectOntoPlane(newContext.GroundNormal);

        var accel = newContext.IsStableOnGround ? stats.MovementAcceleration : stats.AirMovementAcceleration;

        // When over max speed, maintain it if input is in the same direction
        if (Vector2.Dot(Vector2.right * horizontalInput, currentVel) > 0
            && currentVel.magnitude > targetVelocity.magnitude)
        {
            accel = 0;
        }

        var newVel = Vector2.MoveTowards(currentVel,
            targetVelocity,
            accel * Time.fixedDeltaTime);

        var diff = newVel - currentVel;

        dependencies.Rb.velocity = dependencies.Rb.velocity + diff;
    }

    private void CalculateGrounded(MovementStats stats, MovementDependencies dependencies,
        ref MovementContext context)
    {
        // circle cast down for grounded check
        var startPos = dependencies.Rb.position + Vector2.up * stats.GroundCastOffset;
        var hit = Physics2D.CircleCast(
            startPos,
            stats.GroundCastRadius,
            Vector2.down,
            stats.GroundCastDistance,
            stats.groundMask);

        if (hit && forceUngroundedTimer <= 0)
        {
            context.IsGrounded = true;
            context.GroundNormalSmoothed = hit.normal;
            context.GroundNormal = CalculateRawGroundNormal(stats, hit);
            context.IsStableOnGround = Vector2.Angle(Vector2.up, hit.normal) < stats.MaxGroundStableAngle;
        }
        else
        {
            context.IsGrounded = false;
            context.IsStableOnGround = false;
            context.GroundNormalSmoothed = Vector2.up;
            context.GroundNormal = Vector2.up;
        }

        // Debug.DrawLine(startPos, startPos + context.GroundNormalSmoothed, Color.magenta, 1f);
    }

    private Vector2 CalculateRawGroundNormal(MovementStats stats, RaycastHit2D hit)
    {
        // raycast down to the hit point to get the raw ground normal
        var start = hit.centroid + Vector2.up * 0.05f;
        var dir = hit.point + Vector2.up * 0.05f - start;
        var rayHit = Physics2D.Raycast(
            start,
            dir,
            10f,
            stats.groundMask);

        // Debug.DrawLine(start, start + dir, Color.green, 1f);
        // Debug.DrawLine(rayHit.point, rayHit.point + rayHit.normal, Color.blue, 1f);

        return rayHit.normal;
    }

    public void DrawGizmos(MovementStats stats, MovementDependencies dependencies)
    {
        // Draw circle cast
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(dependencies.Rb.position + Vector2.up * stats.GroundCastOffset, stats.GroundCastRadius);
        Gizmos.DrawWireSphere(
            dependencies.Rb.position + Vector2.up * (stats.GroundCastOffset - stats.GroundCastDistance),
            stats.GroundCastRadius);
    }
}