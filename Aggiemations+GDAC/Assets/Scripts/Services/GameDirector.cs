using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField]
    private ServiceContainerSO serviceContainer;

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
        serviceContainer.SceneController.SwitchScenes(SceneController.Scenes.CharacterSelect);
        gameState = GameState.CharacterSelect;
    }

    public void BeginGame()
    {
        if (gameState == GameState.CharacterSelect)
        {
            serviceContainer.SceneController.SwitchScenes(SceneController.Scenes.Fighting);
            gameState = GameState.Fighting;
        }
    }
}