using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class FighterController : MonoBehaviour
{
    private const float ActionStaticTimer = 0.02f;

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

        combatController.OnHitByAttack += HandleOnHitByAttack;

        SubscribeToActionEvents();
    }

    private void ExitDefaultState()
    {
        inputProvider.OnJumpInput -= HandleJumpInput;
        inputProvider.OnTauntInput -= HandleTauntInput;
        inputProvider.OnBasicAttackInput -= HandleBasicAttackInput;
        inputProvider.OnHeavyAttackInput -= HandleHeavyAttackInput;
        inputProvider.OnDashInput -= HandleDashInput;

        combatController.OnHitByAttack -= HandleOnHitByAttack;

        UnsubscribeFromActionEvents();
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
        if (!obj.performed || actionStaticTimer > 0)
        {
            return;
        }

        attackToPerform = AttackType.Heavy;
        SwitchState(State.Action);
    }

    private void HandleBasicAttackInput(InputAction.CallbackContext obj)
    {
        if (!obj.performed || actionStaticTimer > 0)
        {
            return;
        }

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

        var maxFallSpeed = 15f;
        // Limit fall speed
        if (moveDependencies.Rb.velocity.y < -maxFallSpeed)
        {
            var vel = moveDependencies.Rb.velocity;
            vel.y = -maxFallSpeed;
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

        currentActionDirection = ren.flipX ? -1 : 1;

        // Rotate to match normal
        var angle = Vector2.SignedAngle(Vector2.up, moveEngine.Context.GroundNormal) / 2f;
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

        ApplyActionAcceleration();
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

    private void DashStateUpdate()
    {
    }

    private void EnterDashState()
    {
        currentActionDirection = ren.flipX ? -1 : 1;
        anim.Play("Dash");
        dashCooldownTimer = fighterConfig.DashCooldown;
        animEventHandler.OnFinishAction += HandleEndDash;
        combatController.OnHitByAttack += HandleOnHitByAttack;

        combatController.SetInvincible(true);

        SubscribeToActionEvents();
    }

    private void ExitDashState()
    {
        animEventHandler.OnFinishAction -= HandleEndDash;
        combatController.OnHitByAttack -= HandleOnHitByAttack;

        actionStaticTimer = ActionStaticTimer;

        combatController.SetInvincible(false);

        UnsubscribeFromActionEvents();
    }

    private void HandleEndDash()
    {
        SwitchState(State.Default);
    }

    private void DashStateFixedUpdate()
    {
        ApplyActionAcceleration();
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
        currentActionDirection = ren.flipX ? -1 : 1;
        ren.sortingOrder += 1;

        switch (attackToPerform)
        {
            case AttackType.Basic:
                anim.Play("BasicAttack");
                break;
            case AttackType.Heavy:
                anim.Play("HeavyAttack");
                break;
            default:
                break;
        }

        animEventHandler.OnFinishAction += HandleFinishAttack;
        animEventHandler.OnAttackActionImpact += HandleAttackImpact;
        combatController.OnHitByAttack += HandleOnHitByAttack;


        SubscribeToActionEvents();
    }

    private void HandleAttackImpact()
    {
        var hurtbox =
            attackToPerform == AttackType.Basic ? fighterConfig.BasicAttack : fighterConfig.HeavyAttack;

        combatController.PerformAttack(hurtbox, bodyTransformPivot.position, currentActionDirection);
    }

    private void HandleFinishAttack()
    {
        SwitchState(State.Default);
    }

    private void ExitActionState()
    {
        ren.sortingOrder -= 0;
        animEventHandler.OnFinishAction -= HandleFinishAttack;
        animEventHandler.OnAttackActionImpact -= HandleAttackImpact;
        combatController.OnHitByAttack -= HandleOnHitByAttack;

        actionStaticTimer = ActionStaticTimer;

        UnsubscribeFromActionEvents();
    }

    private void ActionStateFixedUpdate()
    {
        ApplyActionAcceleration();
    }

    #endregion

    #region Staggered State

    private void StaggeredStateUpdate()
    {
    }

    private void EnterStaggeredState()
    {
        anim.Play("Staggered");
        moveEngine.ForceUnground(0.1f);
        animEventHandler.OnFinishAction += HandleEndStaggered;
        combatController.OnHitByAttack += HandleOnHitByAttack;
    }

    private void HandleEndStaggered()
    {
        SwitchState(State.Default);
    }

    private void ExitStaggeredState()
    {
        animEventHandler.OnFinishAction -= HandleEndStaggered;
        combatController.OnHitByAttack -= HandleOnHitByAttack;
    }

    private void StaggeredStateFixedUpdate()
    {
        moveEngine.Move(0, moveStats, moveDependencies);
    }

    #endregion

    #region Dead State

    private void DeadStateUpdate()
    {
    }

    private void EnterDeadState()
    {
        anim.Play("Dead");
    }

    private void ExitDeadState()
    {
    }

    private void DeadStateFixedUpdate()
    {
    }

    #endregion
}