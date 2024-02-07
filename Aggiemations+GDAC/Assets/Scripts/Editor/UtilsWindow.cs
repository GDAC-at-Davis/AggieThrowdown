using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class UtilsWindow : EditorWindow
{
    private UtilsPrefs prefs;

    [MenuItem("Window/Utils")]
    public static void ShowWindow()
    {
        GetWindow<UtilsWindow>("Utils");
    }

    private void OnBecameVisible()
    {
        prefs = AssetDatabase.LoadAssetAtPath<UtilsPrefs>("Assets/Scripts/Editor/UtilsPrefs.asset");
        if (prefs == null)
        {
            prefs = CreateInstance<UtilsPrefs>();
            AssetDatabase.CreateAsset(prefs, "Assets/Scripts/Editor/UtilsPrefs.asset");
            AssetDatabase.SaveAssets();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Scene Utils", EditorStyles.boldLabel);

        var startupScenePath = prefs.StartupScenePath;

        if (GUILayout.Button("Play"))
        {
            // Close open editor scenes
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            EditorSceneManager.OpenScene(startupScenePath, OpenSceneMode.Single);
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }
}