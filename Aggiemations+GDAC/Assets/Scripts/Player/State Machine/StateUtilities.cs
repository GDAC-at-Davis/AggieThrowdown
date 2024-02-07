using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FighterController
{
    private void SubscribeToActionEvents()
    {
        animEventHandler.OnMultiplyCurrentVelocityX += HandleMultiplyCurrentVelocityX;
        animEventHandler.OnMultiplyCurrentVelocityY += HandleMultiplyCurrentVelocityY;

        animEventHandler.OnSetActionAccelerationX += HandleSetActionAccelerationX;
        animEventHandler.OnSetActionAccelerationY += HandleSetActionAccelerationY;

        animEventHandler.OnSetActionVelocityX += HandleSetActionVelocityX;
        animEventHandler.OnSetActionVelocityY += HandleSetActionVelocityY;
    }

    private void UnsubscribeFromActionEvents()
    {
        animEventHandler.OnMultiplyCurrentVelocityX -= HandleMultiplyCurrentVelocityX;
        animEventHandler.OnMultiplyCurrentVelocityY -= HandleMultiplyCurrentVelocityY;

        animEventHandler.OnSetActionAccelerationX -= HandleSetActionAccelerationX;
        animEventHandler.OnSetActionAccelerationY -= HandleSetActionAccelerationY;

        animEventHandler.OnSetActionVelocityX -= HandleSetActionVelocityX;
        animEventHandler.OnSetActionVelocityY -= HandleSetActionVelocityY;
    }

    private void ApplyActionAcceleration()
    {
        var vel = moveDependencies.Rb.velocity;
        var xAccel = currentActionAcceleration.x;

        if (xAccel < 0)
        {
            vel.x = Mathf.MoveTowards(vel.x, 0, -xAccel * Time.fixedDeltaTime);
        }
        else
        {
            vel.x += currentActionDirection * xAccel * Time.fixedDeltaTime;
        }

        vel.y += currentActionAcceleration.y * Time.fixedDeltaTime;

        moveDependencies.Rb.velocity = vel;
    }

    private void HandleSetActionVelocityX(float xVel)
    {
        var vel = moveDependencies.Rb.velocity;
        vel.x = xVel * currentActionDirection;
        moveDependencies.Rb.velocity = vel;
    }

    private void HandleSetActionVelocityY(float yVel)
    {
        var vel = moveDependencies.Rb.velocity;
        vel.y = yVel;
        moveDependencies.Rb.velocity = vel;
    }

    private void HandleSetActionAccelerationX(float xAccel)
    {
        currentActionAcceleration.x = xAccel;
    }

    private void HandleSetActionAccelerationY(float yAccel)
    {
        currentActionAcceleration.y = yAccel;
    }

    private void HandleMultiplyCurrentVelocityX(float multiplier)
    {
        var vel = moveDependencies.Rb.velocity;
        vel.x *= multiplier;
        moveDependencies.Rb.velocity = vel;
    }

    private void HandleMultiplyCurrentVelocityY(float multiplier)
    {
        var vel = moveDependencies.Rb.velocity;
        vel.y *= multiplier;
        moveDependencies.Rb.velocity = vel;
    }

    public void HandleOnHitByAttack(FighterCombatController.AttackInstance instance, bool superArmor)
    {
        if (!superArmor)
        {
            SwitchState(State.Staggered);
        }

        // take knockback
        var knockback = instance.attackConfig.KnockbackVelocity;
        knockback.x *= instance.dir;
        moveDependencies.Rb.velocity = knockback;
    }
}