using UnityEngine;

public class FighterManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private ServiceContainerSO serviceContainer;

    [SerializeField]
    private MovementDependencies moveDependencies;

    [SerializeField]
    private FighterController controller;

    [SerializeField]
    private Transform cameraTarget;

    private int playerIndex;
    private FighterConfigSO config;
    private FlexCameraScript flexCamera;

    public void Initialize(int playerIndex, FighterConfigSO config, FlexCameraScript flexCamera)
    {
        this.playerIndex = playerIndex;
        this.config = config;
        this.flexCamera = flexCamera;

        flexCamera.AddTarget(cameraTarget);

        controller.Initialize(playerIndex, serviceContainer.InputManager.GetInputProvider(playerIndex),
            config.MoveStats, moveDependencies);
    }
}