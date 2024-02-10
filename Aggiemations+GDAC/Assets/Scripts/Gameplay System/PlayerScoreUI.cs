using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private TMP_Text nameText;

    public void Initialize(int playerIndex)
    {
        nameText.text = $"Player {playerIndex + 1}";
        nameText.color = ServiceContainerSO.PlayerColors[playerIndex];
    }

    public void UpdateSlider(float value)
    {
        slider.value = value;
    }
}