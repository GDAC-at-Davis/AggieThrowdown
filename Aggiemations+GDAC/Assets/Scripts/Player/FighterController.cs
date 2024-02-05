using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class FighterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private MovementEngine moveEngine;

    [Header("Anim")]
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private SpriteRenderer ren;

    [SerializeField]
    private float rotateSpeed;

    private enum State
    {
        Default,
        Action,
        Staggered,
        Dead
    }

    private State _state;

    private bool jumping;
    private float xInput;

    private InputProvider inputProvider;
    private MovementDependencies moveDepend;
    private MovementStats moveStats;


    public void Initialize(int playerIndex, InputProvider inputProvider, MovementStats stats,
        MovementDependencies moveDependencies)
    {
        _state = State.Default;
        this.inputProvider = inputProvider;
        moveDepend = moveDependencies;
        moveStats = stats;

        inputProvider.OnJumpInput += HandleJumpInput;
    }

    private void OnDestroy()
    {
        inputProvider.OnJumpInput -= HandleJumpInput;
    }

    private void HandleJumpInput(InputAction.CallbackContext ctx)
    {
        var canJump = moveEngine.Context.AirTime < 0.1f || moveEngine.Context.IsStableOnGround;
        if (ctx.started && canJump)
        {
            moveDepend.Rb.velocity = Vector2.right * moveDepend.Rb.velocity.x + Vector2.up * moveStats.JumpVelocity;
            moveEngine.ForceUnground(0.2f);
            jumping = true;
        }

        // Early release
        if (ctx.canceled && jumping)
        {
            jumping = false;
            var velocity = moveDepend.Rb.velocity;
            velocity = new Vector2(velocity.x, velocity.y * 0.65f);
            moveDepend.Rb.velocity = velocity;
        }
    }

    private void Update()
    {
        // Input
        xInput = inputProvider.MovementInput.x;

        if (_state == State.Default)
        {
            if (jumping)
            {
                if (moveDepend.Rb.velocity.y < 0)
                {
                    jumping = false;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (_state == State.Default)
        {
            DefaultModeFixedUpdate();
        }
    }

    private void DefaultModeFixedUpdate()
    {
        moveEngine.Move(xInput, moveStats, moveDepend);

        if (inputProvider.MovementInput.y < 0)
        {
            moveDepend.Rb.velocity += Vector2.down * (moveStats.FastFallAcceleration * Time.deltaTime);
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
        anim.SetFloat("Speed", Mathf.Abs(moveDepend.Rb.velocity.x));
        anim.SetBool("IsGrounded", moveEngine.Context.IsStableOnGround);
    }

    private void OnGUI()
    {
        GUILayout.Label("IsGrounded: " + moveEngine.Context.IsGrounded);
        GUILayout.Label("IsStableOnGround: " + moveEngine.Context.IsStableOnGround);
        GUILayout.Label("GroundNormal: " + moveEngine.Context.GroundNormal);
        GUILayout.Label("Mode: " + _state);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            moveEngine.Move(0, moveStats, moveDepend);
        }

        moveEngine.DrawGizmos(moveStats, moveDepend);
    }
}