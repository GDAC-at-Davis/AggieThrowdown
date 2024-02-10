using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools.Utils;

public class UtilsWindow : EditorWindow
{
    private UtilsPrefs prefs;

    private string newFighterName;

    [MenuItem("GDACAGGIE/GDAGGIE Utilities")]
    public static void ShowWindow()
    {
        GetWindow<UtilsWindow>("GDAGGIE Utilities");
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

        // Divider
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        CreateFighterUI();
    }

    private void CreateFighterUI()
    {
        newFighterName = EditorGUILayout.TextField("New Fighter Name", newFighterName);

        if (GUILayout.Button("Create New Fighter"))
        {
            var newFighterFolderPath = $"{prefs.FighterPath}/{newFighterName}";
            if (!AssetDatabase.IsValidFolder(newFighterFolderPath))
            {
                AssetDatabase.CreateFolder(prefs.FighterPath, newFighterName);
                AssetDatabase.CreateFolder(newFighterFolderPath, "Animations");
            }
            else
            {
                Debug.LogError($"Fighter Already Exists: {newFighterName}");
                return;
            }

            // Copy fighter prefab
            var didCopy = AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(prefs.FighterTemplatePrefab),
                $"{newFighterFolderPath}/{newFighterName}.prefab");

            if (!didCopy)
            {
                Debug.LogError("Failed to create a copy of the fighter template prefab");
                return;
            }

            var newFighterPrefab =
                AssetDatabase.LoadAssetAtPath<FighterManager>(newFighterFolderPath + newFighterName + ".prefab");

            // Create new animation override asset
            var newAnimationOverrides = new AnimatorOverrideController();
            newAnimationOverrides.runtimeAnimatorController =
                prefs.FighterTemplateConfig.AnimationOverrides.runtimeAnimatorController;
            AssetDatabase.CreateAsset(newAnimationOverrides,
                $"{newFighterFolderPath}/{newFighterName}AnimationOverrides.overrideController");

            // Copy portrait texture
            var newPortraitPath = $"{newFighterFolderPath}/{newFighterName}Portrait.png";
            var didCopyPortrait = AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(prefs.FighterPortrait),
                newPortraitPath);
            var newPortrait = AssetDatabase.LoadAssetAtPath<Sprite>(newPortraitPath);

            // Copy fighter config scriptable object and assign new dependencies/values
            var newFighterConfig = Instantiate(prefs.FighterTemplateConfig);
            newFighterConfig.FighterName = newFighterName;
            newFighterConfig.FighterDescription =
                $"{newFighterName} has appeared! Time to create a legendary fighter! (Replace this with your fighter's description)";
            newFighterConfig.IncludeInRoster = true;
            newFighterConfig.FighterPortrait = newPortrait;
            newFighterConfig.AnimationOverrides = newAnimationOverrides;
            AssetDatabase.CreateAsset(newFighterConfig, $"{newFighterFolderPath}/{newFighterName}.asset");

            // Initialize the prefab with new config
            newFighterPrefab.Configure(newFighterConfig);

            AssetDatabase.SaveAssets();
        }
    }
}