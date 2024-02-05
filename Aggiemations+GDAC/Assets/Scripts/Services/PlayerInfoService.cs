using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo
{
    public FighterConfigSO SelectedFighter;
}

public class PlayerInfoService : MonoBehaviour
{
    [SerializeField]
    private ServiceContainerSO serviceContainer;

    private PlayerInfo[] PlayerInfos = new PlayerInfo[69];

    private void Awake()
    {
        serviceContainer.PlayerInfoService = this;
    }

    public void AssignFighterToPlayer(int playerIndex, FighterConfigSO character)
    {
        PlayerInfos[playerIndex].SelectedFighter = character;
    }

    public FighterConfigSO GetFighterForPlayer(int playerIndex)
    {
        return PlayerInfos[playerIndex].SelectedFighter;
    }

    public List<PlayerInfo> GetPlayerInfos()
    {
        var playerInfos = new List<PlayerInfo>();
        for (var i = 0; i < PlayerInfos.Length; i++)
        {
            if (PlayerInfos[i].SelectedFighter != null)
            {
                playerInfos.Add(PlayerInfos[i]);
            }
            else
            {
                break;
            }
        }

        return playerInfos;
    }
}