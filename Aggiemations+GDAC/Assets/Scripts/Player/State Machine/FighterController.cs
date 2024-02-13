using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public partial class FighterController : MonoBehaviour
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

    [Header("Dependencies (Do not modify directly)")]
    [SerializeField]
    private FighterConfigSO fighterConfig;

    [SerializeField]
    private MovementDependencies moveDependencies;

    [SerializeField]
    private AnimationEventHandler animEventHandler;

    [SerializeField]
    private FighterCombatController combatController;

    [SerializeField]
    private Transform bodyTransformPivot;


    private enum State
    {
        Stasis,
        Default,
        Dash,
        Action,
        Staggered
    }

    private MovementStats moveStats => fighterConfig.MoveStats;

    private State currentState = State.Stasis;

    private bool jumping;
    private float xInput;

    private InputProvider inputProvider;
    private int playerIndex;

    // Action state
    private Vector2 currentActionAcceleration;
    private int currentActionDirection;

    // Timers
    private float dashCooldownTimer;
    private float actionStaticTimer;
    private float jumpStaticTimer;

    public void Configure(MovementStats stats,
        MovementDependencies moveDependencies,
        FighterConfigSO config,
        AnimationEventHandler animEventHandler,
        FighterCombatController combatController,
        Transform bodyTransformPivot)
    {
        this.moveDependencies = moveDependencies;
        fighterConfig = config;
        this.animEventHandler = animEventHandler;
        this.combatController = combatController;
        this.bodyTransformPivot = bodyTransformPivot;

        anim.runtimeAnimatorController = config.AnimationOverrides;
    }

    public void Initialize(int playerIndex,
        InputProvider inputProvider)
    {
        currentState = State.Default;
        this.inputProvider = inputProvider;
        this.playerIndex = playerIndex;
        anim.Play("EntryFlex");
    }

    private void OnDestroy()
    {
        SwitchState(State.Stasis);
    }

    private void Update()
    {
        // Timers
        dashCooldownTimer -= Time.deltaTime;
        actionStaticTimer -= Time.deltaTime;
        jumpStaticTimer -= Time.deltaTime;

        // Input
        xInput = inputProvider.MovementInput.x;

        switch (currentState)
        {
            case State.Default:
                DefaultStateUpdate();
                break;
            case State.Dash:
                DashStateUpdate();
                break;
            case State.Action:
                ActionStateUpdate();
                break;
            case State.Staggered:
                StaggeredStateUpdate();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Stasis:
                moveEngine.Move(0, moveStats, moveDependencies);
                break;
            case State.Default:
                DefaultStateFixedUpdate();
                break;
            case State.Dash:
                DashStateFixedUpdate();
                break;
            case State.Action:
                ActionStateFixedUpdate();
                break;
            case State.Staggered:
                StaggeredStateFixedUpdate();
                break;
        }
    }

    private void SwitchState(State newState)
    {
        // Reset action state
        currentActionAcceleration = Vector2.zero;

        // flip ren
        if (xInput > 0)
        {
            ren.flipX = false;
        }
        else if (xInput < 0)
        {
            ren.flipX = true;
        }

        switch (currentState)
        {
            case State.Default:
                ExitDefaultState();
                break;
            case State.Dash:
                ExitDashState();
                break;
            case State.Action:
                ExitActionState();
                break;
            case State.Staggered:
                ExitStaggeredState();
                break;
        }

        currentState = newState;

        switch (currentState)
        {
            case State.Default:
                EnterDefaultState();
                break;
            case State.Dash:
                EnterDashState();
                break;
            case State.Action:
                EnterActionState();
                break;
            case State.Staggered:
                EnterStaggeredState();
                break;
        }
    }

    private void OnGUI()
    {
        return;

        GUILayout.BeginArea(new Rect(10 + 500 * playerIndex, 10, 300, 300));
        GUILayout.Label("IsGrounded: " + moveEngine.Context.IsGrounded);
        GUILayout.Label("IsStableOnGround: " + moveEngine.Context.IsStableOnGround);
        GUILayout.Label("GroundNormal: " + moveEngine.Context.GroundNormal);
        GUILayout.Label("Mode: " + currentState);
        GUILayout.Label("Invincible: " + combatController.IsInvincible);
        GUILayout.EndArea();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // moveEngine.Move(0, moveStats, moveDependencies);
        }

        moveEngine.DrawGizmos(moveStats, moveDependencies);
    }

    public void FlipSprite(bool value)
    {
        ren.flipX = value;
    }

    public void EnableControl(bool val)
    {
        if (val)
        {
            SwitchState(State.Default);
        }
        else
        {
            SwitchState(State.Stasis);
        }
    }
}