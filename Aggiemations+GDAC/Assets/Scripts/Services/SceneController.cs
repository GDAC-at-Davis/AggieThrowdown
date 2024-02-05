using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    private ServiceContainerSO container;

    [SerializeField]
    private List<string> SceneNames;

    public enum Scenes
    {
        CharacterSelect,
        Fighting
    }

    private string currentScene;

    private void Awake()
    {
        container.SceneController = this;
        currentScene = string.Empty;
    }

    public void SwitchScenes(Scenes scene)
    {
        var sceneName = SceneNames[(int)scene];
        if (currentScene != string.Empty)
        {
            SceneManager.UnloadSceneAsync(currentScene);
        }

        var handler = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        handler.completed += (AsyncOperation op) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        };

        currentScene = sceneName;
    }
}