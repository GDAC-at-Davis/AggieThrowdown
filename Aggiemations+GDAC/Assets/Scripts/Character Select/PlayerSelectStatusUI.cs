using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSelectStatusUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text readyText;

    [SerializeField]
    private TMP_Text playerText;

    public void Initialize(int playerIndex)
    {
        playerText.text = "Player " + (playerIndex + 1);
    }

    public void SetReadyStatus(bool isReady)
    {
        readyText.text = isReady ? "Ready" : "Not Ready";
    }
}