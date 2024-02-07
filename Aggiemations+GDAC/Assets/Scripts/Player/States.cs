using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class FighterController : MonoBehaviour
{
    #region Default State

    private void DefaultStateUpdate()
    {
        if (jumping)
        {
            if (moveDependencies.Rb.velocity.y < 0)
            {
                jumping = false;
            }
        }
    }

    private void EnterDefaultState()
    {
        inputProvider.OnJumpInput += HandleJumpInput;
        inputProvider.OnTauntInput += HandleTauntInput;
        inputProvider.OnBasicAttackInput += HandleBasicAttackInput;
        inputProvider.OnHeavyAttackInput += HandleHeavyAttackInput;
        inputProvider.OnDashInput += HandleDashInput;
    }

    private void ExitDefaultState()
    {
        inputProvider.OnJumpInput -= HandleJumpInput;
        inputProvider.OnBasicAttackInput -= HandleBasicAttackInput;
        inputProvider.OnHeavyAttackInput -= HandleHeavyAttackInput;
        inputProvider.OnDashInput -= HandleDashInput;
    }

    private void HandleDashInput(InputAction.CallbackContext obj)
    {
        if (dashCooldownTimer > 0)
        {
            return;
        }

        SwitchState(State.Dash);
    }

    private void HandleHeavyAttackInput(InputAction.CallbackContext obj)
    {
        attackToPerform = AttackType.Heavy;
        SwitchState(State.Action);
    }

    private void HandleBasicAttackInput(InputAction.CallbackContext obj)
    {
        attackToPerform = AttackType.Basic;
        SwitchState(State.Action);
    }

    private void DefaultStateFixedUpdate()
    {
        moveEngine.Move(xInput, moveStats, moveDependencies);

        // Fastfall
        if (!moveEngine.Context.IsGrounded && inputProvider.MovementInput.y < 0 &&
            moveDependencies.Rb.velocity.y > -moveStats.FastFallVelocity)
        {
            var vel = moveDependencies.Rb.velocity;
            vel.y = -moveStats.FastFallVelocity;
            moveDependencies.Rb.velocity = vel;
        }

        // flip ren
        if (xInput > 0)
        {
            ren.flipX = false;
        }
        else if (xInput < 0)
        {
            ren.flipX = true;
        }

        // Rotate to match normal
        var angle = Vector2.SignedAngle(Vector2.up, moveEngine.Context.GroundNormal);
        var newAngle =
            Mathf.MoveTowardsAngle(ren.transform.rotation.eulerAngles.z, angle, Time.deltaTime * rotateSpeed);
        ren.transform.rotation = Quaternion.Euler(0, 0, newAngle);

        // anims
        if (moveEngine.Context.IsStableOnGround)
        {
            if (moveDependencies.Rb.velocity.x != 0)
            {
                anim.Play("Run");
            }
            else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Taunt"))
            {
                anim.Play("Idle");
            }
        }
        else
        {
            if (moveDependencies.Rb.velocity.y < 0)
            {
                anim.Play("Falling");
            }
            else if (moveDependencies.Rb.velocity.y > 0)
            {
                anim.Play("Jump");
            }
        }
    }


    private void HandleTauntInput(InputAction.CallbackContext obj)
    {
        if (obj.started && inputProvider.MovementInput.x == 0)
        {
            anim.Play("Taunt");
        }
    }


    private void HandleJumpInput(InputAction.CallbackContext ctx)
    {
        var canJump = moveEngine.Context.AirTime < 0.1f || moveEngine.Context.IsStableOnGround;

        if (ctx.started && canJump)
        {
            moveDependencies.Rb.velocity =
                Vector2.right * moveDependencies.Rb.velocity.x + Vector2.up * moveStats.JumpVelocity;
            moveEngine.ForceUnground(0.2f);
            jumping = true;
        }

        // Early release
        if (ctx.canceled && jumping)
        {
            jumping = false;
            var velocity = moveDependencies.Rb.velocity;
            velocity = new Vector2(velocity.x, velocity.y * 0.65f);
            moveDependencies.Rb.velocity = velocity;
        }
    }

    #endregion

    #region Dash State

    private int dashDirection;

    private void DashStateUpdate()
    {
    }

    private void EnterDashState()
    {
        dashDirection = ren.flipX ? -1 : 1;
        anim.Play("Dash");
        dashCooldownTimer = fighterConfig.DashCooldown;
        animEventHandler.OnBeginDash += HandleBeginDash;
        animEventHandler.OnFinishAction += HandleEndDash;
    }

    private void ExitDashState()
    {
        animEventHandler.OnBeginDash -= HandleBeginDash;
        animEventHandler.OnFinishAction -= HandleEndDash;
    }

    private void HandleBeginDash()
    {
        moveDependencies.Rb.velocity = new Vector2(dashDirection * fighterConfig.DashVelocity, 0);
    }

    private void HandleEndDash()
    {
        SwitchState(State.Default);
    }

    private void DashStateFixedUpdate()
    {
        var vel = moveDependencies.Rb.velocity;
        var accel = fighterConfig.DashAccelerationOverTime;

        if (accel < 0)
        {
            vel.x = Mathf.MoveTowards(vel.x, 0, -accel * Time.fixedDeltaTime);
        }
        else
        {
            vel.x += dashDirection * fighterConfig.DashAccelerationOverTime * Time.fixedDeltaTime;
        }

        moveDependencies.Rb.velocity = vel;
    }

    #endregion

    #region Action State

    private enum AttackType
    {
        Basic,
        Heavy
    }

    private AttackType attackToPerform;

    private void ActionStateUpdate()
    {
    }

    private void EnterActionState()
    {
    }

    private void ExitActionState()
    {
    }

    private void ActionStateFixedUpdate()
    {
    }

    #endregion

    #region Staggered State

    private void StaggeredStateUpdate()
    {
    }

    private void EnterStaggeredState()
    {
    }

    private void ExitStaggeredState()
    {
    }

    private void StaggeredStateFixedUpdate()
    {
    }

    #endregion

    #region Dead State

    private void DeadStateUpdate()
    {
    }

    private void EnterDeadState()
    {
    }

    private void ExitDeadState()
    {
    }

    private void DeadStateFixedUpdate()
    {
    }

    #endregion
}