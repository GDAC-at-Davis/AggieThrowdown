using System;
using UnityEngine;
using UnityEngine.Serialization;

public class FighterManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private FighterConfigSO config;

    [Header("Dependencies (Do not modify in fighter variants)")]
    [SerializeField]
    private ServiceContainerSO serviceContainer;

    [SerializeField]
    private MovementDependencies movementDependencies;

    [SerializeField]
    private FighterController controller;

    [SerializeField]
    private FighterCombatEntity combatEntity;

    [SerializeField]
    private AnimationEventHandler animEventHandler;

    [SerializeField]
    private Transform bodyTransformPivot;

    private int playerIndex;
    private FlexCameraScript flexCamera;

    [ContextMenu("Initialize In Editor")]
    public void OnValidate()
    {
        if (!config)
        {
            return;
        }

        Configure();
    }

    private void Configure()
    {
        controller.Configure(config.MoveStats, movementDependencies, config, animEventHandler);
    }

    public void Initialize(int playerIndex, FighterConfigSO config, FlexCameraScript flexCamera)
    {
        this.playerIndex = playerIndex;
        this.config = config;
        this.flexCamera = flexCamera;

        if (this.flexCamera)
        {
            flexCamera.AddTarget(bodyTransformPivot);
        }

        controller.Initialize(playerIndex, serviceContainer.InputManager.GetInputProvider(playerIndex));

        // Hook up events
        animEventHandler.OnSetInvincible += combatEntity.SetInvincible;
        animEventHandler.OnSetSuperArmor += combatEntity.SetSuperArmor;
    }

    public Vector2 GetBodyPosition()
    {
        return bodyTransformPivot.position;
    }

    private void OnDrawGizmos()
    {
        // draw hurtboxes
        Gizmos.color = Color.green;
        config.BasicAttack.DrawHurtboxGizmos(bodyTransformPivot.position, 1);

        Gizmos.color = Color.blue;
        config.HeavyAttack.DrawHurtboxGizmos(bodyTransformPivot.position, 1);
    }
}