using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilsPrefs : ScriptableObject
{
    [field: SerializeField]
    [field: TextArea]
    public string StartupScenePath { get; private set; }

    [field: SerializeField]
    public FighterManager FighterTemplatePrefab { get; private set; }

    [field: SerializeField]
    public FighterConfigSO FighterTemplateConfig { get; private set; }
}