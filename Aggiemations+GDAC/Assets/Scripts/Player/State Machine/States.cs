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


        // anims
        if (moveEngine.Context.IsStableOnGround)
        {
            if (Mathf.Abs(moveDependencies.Rb.velocity.x) > 1f)
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
                anim.Play("Fall");
            }
            else if (moveDependencies.Rb.velocity.y > 0)
            {
                anim.Play("Jump");
            }
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

    private void HandleDashInput(InputProvider.InputContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started || dashCooldownTimer > 0)
        {
            return;
        }

        if (ctx.direction != 0)
        {
            xInput = ctx.direction;
        }

        SwitchState(State.Dash);
    }

    private void HandleHeavyAttackInput(InputProvider.InputContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started || actionStaticTimer > 0)
        {
            return;
        }

        attackToPerform = AttackType.Heavy;
        SwitchState(State.Action);
    }

    private void HandleBasicAttackInput(InputProvider.InputContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started || actionStaticTimer > 0)
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
        if (!moveEngine.Context.IsGrounded
            && inputProvider.MovementInput.y < -0.4f
            && moveDependencies.Rb.velocity.y > -moveStats.FastFallVelocity
            && !jumping
            && jumpStaticTimer < 0)
        {
            var vel = moveDependencies.Rb.velocity;
            vel.y = -moveStats.FastFallVelocity;
            moveDependencies.Rb.velocity = vel;
        }

        if (moveEngine.Context.IsGrounded)
        {
            didJump = false;
        }

        var maxFallSpeed = 15f;
        // Limit fall speed
        if (moveDependencies.Rb.velocity.y < -maxFallSpeed)
        {
            var vel = moveDependencies.Rb.velocity;
            vel.y = -maxFallSpeed;
            moveDependencies.Rb.velocity = vel;
        }

        // Rotate to match normal
        var angle = Vector2.SignedAngle(Vector2.up, moveEngine.Context.GroundNormal) / 2f;
        var newAngle =
            Mathf.MoveTowardsAngle(ren.transform.rotation.eulerAngles.z, angle, Time.deltaTime * rotateSpeed);
        ren.transform.rotation = Quaternion.Euler(0, 0, newAngle);

        ApplyActionAcceleration();
    }


    private void HandleTauntInput(InputProvider.InputContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && Mathf.Abs(moveDependencies.Rb.velocity.x) < 1f)
        {
            anim.Play("Taunt");
            SFXOneshotPlayer.Instance.PlaySFXOneshot(bodyTransformPivot.position, fighterConfig.AudioConfig.TauntClip);
        }
    }


    private void HandleJumpInput(InputProvider.InputContext ctx)
    {
        var coyoteTime = Time.time - moveEngine.Context.LastGroundedTime < moveStats.CoyoteTime;
        var canJump = !jumping && jumpStaticTimer < 0f && !didJump;

        if (ctx.phase == InputActionPhase.Started && canJump)
        {
            moveDependencies.Rb.velocity =
                Vector2.right * moveDependencies.Rb.velocity.x + Vector2.up * moveStats.JumpVelocity;
            moveEngine.ForceUnground(0.2f);
            SFXOneshotPlayer.Instance.PlaySFXOneshot(bodyTransformPivot.position, fighterConfig.AudioConfig.JumpClip);
            jumping = true;
            jumpStaticTimer = 0.2f;
            didJump = true;
        }

        // Early release
        if (ctx.phase == InputActionPhase.Canceled && jumping)
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
        anim.Play("Dash", -1, 0);
        dashFixedUpdateCounter = 0;
        animEventHandler.OnFinishAction += HandleEndDash;
        combatController.OnHitByAttack += HandleOnHitByAttack;

        combatController.SetInvincible(true);
        SFXOneshotPlayer.Instance.PlaySFXOneshot(bodyTransformPivot.position, fighterConfig.AudioConfig.DashClip);

        SubscribeToActionEvents();
    }

    private void ExitDashState()
    {
        animEventHandler.OnFinishAction -= HandleEndDash;
        combatController.OnHitByAttack -= HandleOnHitByAttack;
        dashCooldownTimer = fighterConfig.DashCooldown;

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
        // make sure that the first afterimage is on the second frame to avoid the pre-dash sprite showing
        dashFixedUpdateCounter++;
        if (dashFixedUpdateCounter % 2 == 0)
        {
            OnAfterImageRequested?.Invoke(ren.transform, ren.flipX, ren.sprite);
        }
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
                anim.Play("BasicAttack", -1, 0);
                break;
            case AttackType.Heavy:
                anim.Play("HeavyAttack", -1, 0);
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

        var result = combatController.PerformAttack(hurtbox, bodyTransformPivot.position, currentActionDirection);

        if (result)
        {
            SFXOneshotPlayer.Instance.PlaySFXOneshot(bodyTransformPivot.position,
                attackToPerform == AttackType.Basic
                    ? fighterConfig.AudioConfig.BasicAttackImpactClip
                    : fighterConfig.AudioConfig.HeavyAttackImpactClip);
        }
        else
        {
            SFXOneshotPlayer.Instance.PlaySFXOneshot(bodyTransformPivot.position,
                attackToPerform == AttackType.Basic
                    ? fighterConfig.AudioConfig.BasicAttackClip
                    : fighterConfig.AudioConfig.HeavyAttackClip);
        }
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
        anim.Play("Staggered", -1, 0);
        anim.Update(0.1f);
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
}