using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterEntryUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text nameText;

    [SerializeField]
    private TMP_Text selectedText;

    [SerializeField]
    private Image fighterPortraitImage;

    private List<int> selectedBy = new();

    public void Initialize(FighterConfigSO fighterConfig)
    {
        nameText.text = fighterConfig.FighterName;
        fighterPortraitImage.sprite = fighterConfig.FighterPortrait;
        UpdateSelectedByText();
    }

    public void SelectedBy(int playerIndex)
    {
        selectedBy.Add(playerIndex);
        UpdateSelectedByText();
    }

    public void UnselectedBy(int playerIndex)
    {
        selectedBy.Remove(playerIndex);
        UpdateSelectedByText();
    }

    private void UpdateSelectedByText()
    {
        var text = new StringBuilder();
        for (var i = 0; i < selectedBy.Count; i++)
        {
            text.Append($"P{selectedBy[i] + 1}");
            if (i < selectedBy.Count - 1)
            {
                text.Append(", ");
            }
        }

        selectedText.text = text.ToString();
    }
}