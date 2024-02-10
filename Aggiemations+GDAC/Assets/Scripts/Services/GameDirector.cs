using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField]
    private ServiceContainerSO serviceContainer;

    [SerializeField]
    private SceneController sceneController;

    private enum GameState
    {
        CharacterSelect,
        Fighting
    }

    private GameState gameState;

    private void Awake()
    {
        serviceContainer.GameDirector = this;
    }

    private void Start()
    {
        sceneController.SwitchScenes(SceneController.Scenes.CharacterSelect);
        gameState = GameState.CharacterSelect;
    }

    public void BeginGame()
    {
        if (gameState == GameState.CharacterSelect)
        {
            sceneController.SwitchScenes(SceneController.Scenes.Fighting);
            gameState = GameState.Fighting;
        }
    }

    public void FinishGame()
    {
        if (gameState == GameState.Fighting)
        {
            sceneController.SwitchScenes(SceneController.Scenes.CharacterSelect);
            gameState = GameState.CharacterSelect;
        }
    }
}