using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Roster SO", menuName = "Fighter Roster SO")]
public class RosterSO : ScriptableObject
{
    [field: SerializeField]
    public List<FighterConfigSO> FighterRoster { get; private set; }

    public void OnValidate()
    {
#if UNITY_EDITOR
        // find all the SO of type FighterConfigSO in assets
        var fighterConfigsPaths = AssetDatabase.FindAssets("t:FighterConfigSO");

        FighterRoster.RemoveAll((a) => a == null || !a.IncludeInRoster);

        foreach (var path in fighterConfigsPaths)
        {
            var newConfig = AssetDatabase.LoadAssetAtPath<FighterConfigSO>(AssetDatabase.GUIDToAssetPath(path));
            if (newConfig != null && !FighterRoster.Contains(newConfig) && newConfig.IncludeInRoster)
            {
                FighterRoster.Add(newConfig);
            }
        }
#endif
    }
}