using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchDirector : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private ServiceContainerSO serviceContainer;

    [SerializeField]
    private FlexCameraScript flexCamera;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private FighterManager fighterPrefab;

    private List<PlayerInfo> playerInfos;

    private List<FighterManager> fighters = new();

    private void Start()
    {
        playerInfos = serviceContainer.PlayerInfoService.GetPlayerInfos();

        // Spawn players
        for (var i = 0; i < playerInfos.Count; i++)
        {
            var playerInfo = playerInfos[i];
            var fighterConfig = playerInfo.SelectedFighter;

            var fighter = Instantiate(fighterPrefab, spawnPoint.position, Quaternion.identity);
            fighter.Initialize(i, fighterConfig, flexCamera);

            fighters.Add(fighter);
        }
    }
}