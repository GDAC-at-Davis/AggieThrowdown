using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerSelectStatusUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text readyText;

    [SerializeField]
    private TMP_Text playerText;

    [SerializeField]
    private Image characterImage;

    [SerializeField]
    private TMP_Text characterNameText;

    [SerializeField]
    private TMP_Text characterDescriptionText;

    public void Initialize(int playerIndex)
    {
        playerText.text = "Player " + (playerIndex + 1);
    }

    public void SetReadyStatus(bool isReady)
    {
        readyText.color = isReady ? Color.green : Color.red;
        readyText.text = isReady ? "Ready" : "Not Ready";
    }

    public void SetCharacter(FighterConfigSO config)
    {
        characterImage.sprite = config.FighterPortrait;
        characterNameText.text = config.FighterName;
        characterDescriptionText.text = config.FighterDescription;
    }
}