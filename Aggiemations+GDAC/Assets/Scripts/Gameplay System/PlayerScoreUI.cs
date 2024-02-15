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

    [SerializeField]
    private SimpleAnimator simpleAnimator;

    public void Initialize(int playerIndex)
    {
        nameText.text = $"Player {playerIndex + 1}";
        nameText.color = ServiceContainerSO.PlayerColors[playerIndex];
    }

    public void UpdateSlider(float value)
    {
        if (slider.value > value)
        {
            simpleAnimator.Play(true);
        }

        slider.value = value;
    }

    public void SetFlipX(bool val)
    {
        var lScale = transform.localScale;
        lScale.x = val ? -1 : 1;
        transform.localScale = lScale;

        nameText.transform.localScale = lScale;
    }
}