using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Roster SO", menuName = "Fighter Roster SO")]
public class RosterSO : ScriptableObject
{
    [field: SerializeField]
    public List<FighterConfigSO> FighterRoster { get; private set; }
}