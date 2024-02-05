using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class UtilsWindow : EditorWindow
{
    private string startupScenePath;

    [MenuItem("Window/Utils")]
    public static void ShowWindow()
    {
        GetWindow<UtilsWindow>("Utils");
    }

    private void OnBecameVisible()
    {
        startupScenePath = EditorPrefs.GetString("StartupScenePath");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Scene Utils", EditorStyles.boldLabel);

        startupScenePath = EditorGUILayout.TextField("Startup Scene Path", startupScenePath);

        EditorPrefs.SetString("StartupScenePath", startupScenePath);

        if (GUILayout.Button("Play"))
        {
            // Close open editor scenes
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            EditorSceneManager.OpenScene(startupScenePath, OpenSceneMode.Single);
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }
}