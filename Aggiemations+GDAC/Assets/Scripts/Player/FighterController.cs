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


    private enum State
    {
        NoState,
        Default,
        Dash,
        Action,
        Staggered,
        Dead
    }

    private MovementStats moveStats => fighterConfig.MoveStats;

    private State currentState = State.NoState;

    private bool jumping;
    private float xInput;

    private InputProvider inputProvider;
    private int playerIndex;

    // Timers
    private float dashCooldownTimer;

    public void Configure(MovementStats stats, MovementDependencies moveDependencies,
        FighterConfigSO config, AnimationEventHandler animEventHandler)
    {
        this.moveDependencies = moveDependencies;
        fighterConfig = config;
        this.animEventHandler = animEventHandler;

        anim.runtimeAnimatorController = config.AnimationOverrides;
    }

    public void Initialize(int playerIndex,
        InputProvider inputProvider)
    {
        currentState = State.Default;
        this.inputProvider = inputProvider;
        this.playerIndex = playerIndex;
        SwitchState(State.Default);
    }

    private void OnDestroy()
    {
        SwitchState(State.NoState);
    }

    private void Update()
    {
        // Timers
        dashCooldownTimer -= Time.deltaTime;

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
            case State.Dead:
                DeadStateUpdate();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
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
            case State.Dead:
                DeadStateFixedUpdate();
                break;
        }
    }

    private void SwitchState(State newState)
    {
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
            case State.Dead:
                ExitDeadState();
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
            case State.Dead:
                EnterDeadState();
                break;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10 + 500 * playerIndex, 10, 300, 300));
        GUILayout.Label("IsGrounded: " + moveEngine.Context.IsGrounded);
        GUILayout.Label("IsStableOnGround: " + moveEngine.Context.IsStableOnGround);
        GUILayout.Label("GroundNormal: " + moveEngine.Context.GroundNormal);
        GUILayout.Label("Mode: " + currentState);
        GUILayout.EndArea();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            moveEngine.Move(0, moveStats, moveDependencies);
        }

        moveEngine.DrawGizmos(moveStats, moveDependencies);
    }
}