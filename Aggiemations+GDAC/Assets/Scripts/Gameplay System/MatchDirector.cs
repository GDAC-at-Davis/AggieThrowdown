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
    private List<Transform> spawnPoint;

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

            var fighter = Instantiate(fighterConfig.FighterPrefab, spawnPoint[i].position, Quaternion.identity);
            fighter.Initialize(i, fighterConfig, flexCamera);

            fighters.Add(fighter);
        }
    }

    private void RespawnPlayer()
    {
        // get respawn position furthest from all players
        var respawnPosition = Vector3.zero;
        var maxDistance = float.MinValue;
        foreach (var point in spawnPoint)
        {
            var minDistance = float.MaxValue;
            foreach (var fighter in fighters)
            {
                var distance = Vector3.Distance(point.position, fighter.GetBodyPosition());
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }

            if (minDistance > maxDistance)
            {
                maxDistance = minDistance;
                respawnPosition = point.position;
            }
        }
    }
}