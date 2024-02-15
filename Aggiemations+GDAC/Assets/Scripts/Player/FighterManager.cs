using System;
using TMPro;
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
    private FighterCombatController combatController;

    [SerializeField]
    private FighterHitbox hitbox;

    [SerializeField]
    private AnimationEventHandler animEventHandler;

    [SerializeField]
    private Transform bodyTransformPivot;

    [SerializeField]
    private TMP_Text nameText;

    private int playerIndex;
    private MainCameraController mainCamera;

    [ContextMenu("Initialize In Editor")]
    public void OnValidate()
    {
        if (!config)
        {
            return;
        }

        Configure();
    }

    public void Configure(FighterConfigSO newConfig)
    {
        config = newConfig;
        controller.Configure(config.MoveStats, movementDependencies, config, animEventHandler, combatController,
            bodyTransformPivot);
    }

    private void Configure()
    {
        controller.Configure(config.MoveStats, movementDependencies, config, animEventHandler, combatController,
            bodyTransformPivot);
    }

    public void Initialize(int playerIndex, FighterConfigSO config, Vector3 spawnPosition,
        MainCameraController mainCamera)
    {
        Configure(this.config);

        this.playerIndex = playerIndex;
        this.config = config;
        this.mainCamera = mainCamera;

        if (this.mainCamera)
        {
            mainCamera.AddTarget(bodyTransformPivot);
        }

        movementDependencies.Rb.transform.position = spawnPosition;

        controller.Initialize(playerIndex, serviceContainer.InputManager.GetInputProvider(playerIndex));
        combatController.Initialize(config, playerIndex, hitbox);

        nameText.text = $"{playerIndex + 1}";
        nameText.color = ServiceContainerSO.PlayerColors[playerIndex];

        // Hook up events
        animEventHandler.OnSetSuperArmor += combatController.SetSuperArmor;
        combatController.OnHitByAttack += OnHitByAttack;
    }

    public void SetControl(bool val)
    {
        controller.EnableControl(val);
    }

    private void OnHitByAttack(FighterCombatController.AttackInstance attackInstance, bool superArmor)
    {
        serviceContainer.EventManager.OnPlayerHitByAttack?.Invoke(playerIndex, attackInstance);
    }

    public Vector2 GetBodyPosition()
    {
        return bodyTransformPivot.position;
    }

    private void OnDrawGizmos()
    {
        // draw hurtboxes in editor mode
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.green;
            config.BasicAttack.DrawHurtboxGizmos(bodyTransformPivot.position, 1);

            Gizmos.color = Color.magenta;
            config.HeavyAttack.DrawHurtboxGizmos(bodyTransformPivot.position, 1);
        }
    }

    public void FlipSprite(bool value)
    {
        controller.FlipSprite(value);
    }
}