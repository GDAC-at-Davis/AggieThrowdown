using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MatchDirector : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private ServiceContainerSO serviceContainer;

    [FormerlySerializedAs("flexCamera")]
    [SerializeField]
    private MainCameraController mainCamera;

    [SerializeField]
    private ArenaMap map;

    private List<PlayerInfo> playerInfos;

    private List<FighterManager> fighters = new();

    private void Start()
    {
        map.SetArenaCameraActive(true);
        StartCoroutine(StartMatchCoroutine());
    }

    private IEnumerator StartMatchCoroutine()
    {
        playerInfos = serviceContainer.PlayerInfoService.GetPlayerInfos();

        // Fast spawning for testing
        float delayMult = 1;
        foreach (var playerInfo in playerInfos)
        {
            if (playerInfo.SelectedFighterConfig.FastLoadForTest)
            {
                delayMult = 0;
            }
        }

        var spawnPoints = map.SpawnPoints;

        map.SetArenaCameraActive(true);

        yield return new WaitForSecondsRealtime(1.5f * delayMult);

        map.SetArenaCameraActive(false);

        for (var i = 0; i < playerInfos.Count; i++)
        {
            // Zoom in on spawn point
            spawnPoints[i].SetCameraActive(true);
            yield return new WaitForSecondsRealtime(1.25f * delayMult);

            // Spawn player
            var playerInfo = playerInfos[i];
            var fighterConfig = playerInfo.SelectedFighterConfig;

            var fighter = Instantiate(fighterConfig.FighterPrefab, Vector3.zero, Quaternion.identity);
            fighter.Initialize(i, fighterConfig, spawnPoints[i].GetPosition(), mainCamera);

            fighter.FlipSprite(spawnPoints[i].FlipSprite);

            fighter.SetControl(false);

            yield return new WaitForSecondsRealtime(2.5f * delayMult);

            fighters.Add(fighter);
            spawnPoints[i].SetCameraActive(false);
        }

        yield return new WaitForSeconds(1.5f * delayMult);

        // Begin match
        foreach (var fighter in fighters)
        {
            fighter.SetControl(true);
        }
    }
}