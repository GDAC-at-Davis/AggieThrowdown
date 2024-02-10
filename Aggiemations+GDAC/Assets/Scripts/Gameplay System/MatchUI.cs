using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchUI : MonoBehaviour
{
    [Header("Start and End UI")]
    [SerializeField]
    private CanvasGroup matchStartScreen;

    [SerializeField]
    private SimpleAnimator matchStartAnimator;

    [SerializeField]
    private CanvasGroup matchEndScreen;

    [SerializeField]
    private SimpleAnimator matchEndAnimator;

    [SerializeField]
    private TMP_Text matchEndText;

    [Header("Score UI")]
    [SerializeField]
    private PlayerScoreUI playerScorePrefab;

    [SerializeField]
    private HorizontalLayoutGroup playerScoreLayout;

    private void Start()
    {
        matchStartScreen.alpha = 0;
        matchEndScreen.alpha = 0;
    }

    public void ShowMatchStartScreen()
    {
        matchStartAnimator.Play();
    }

    public void ShowMatchEndScreen(int winningPlayerIndex)
    {
        matchEndText.text = $"Player {winningPlayerIndex + 1} wins!";
        matchEndAnimator.Play();
    }
}