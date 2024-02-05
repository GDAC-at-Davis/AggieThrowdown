using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField]
    private ServiceContainerSO serviceContainer;

    [SerializeField]
    private RosterSO roster;

    [Header("UI")]
    [SerializeField]
    private CharacterEntryUI characterEntryPrefab;

    [FormerlySerializedAs("playerFighterSelectStatusPrefab")]
    [FormerlySerializedAs("playerStatusPrefab")]
    [SerializeField]
    private PlayerSelectStatusUI playerSelectStatusPrefab;

    [SerializeField]
    private GridLayoutGroup CharacterEntryLayoutParent;

    [SerializeField]
    private GridLayoutGroup PlayerStatusLayoutParent;

    private MultiInputManager inputManager;

    private List<CharacterEntryUI> characterEntriesUI = new();
    private List<PlayerCharacterSelector> playerSelections = new();

    private void Start()
    {
        inputManager = serviceContainer.InputManager;

        // Initialize character entries UI
        for (var i = 0; i < roster.FighterRoster.Count; i++)
        {
            var entry = Instantiate(characterEntryPrefab, CharacterEntryLayoutParent.transform);
            entry.Initialize(roster.FighterRoster[i]);
            characterEntriesUI.Add(entry);
        }

        // Initialize each player's character selector
        playerSelections = new List<PlayerCharacterSelector>();

        for (var i = 0; i < 2; i++)
        {
            var inputProvider = inputManager.GetInputProvider(i);

            var playerStatusUI = Instantiate(playerSelectStatusPrefab, PlayerStatusLayoutParent.transform);
            playerStatusUI.Initialize(i);

            var selector = new PlayerCharacterSelector(i, inputProvider, playerStatusUI);
            playerSelections.Add(selector);

            characterEntriesUI[0].SelectedBy(i);

            // Events for selection actions
            selector.ChangeCharacter += HandleChangeCharacter;
            selector.ReadyUp += HandleReadyUp;
        }
    }

    private void HandleReadyUp(int playerIndex)
    {
        // Check if all players are ready
        var allReady = true;
        for (var i = 0; i < playerSelections.Count; i++)
        {
            if (!playerSelections[i].isReady)
            {
                allReady = false;
                break;
            }
        }

        if (allReady)
        {
            FinishCharacterSelect();
        }
    }

    private void FinishCharacterSelect()
    {
        // Assign chosen fighters to players
        for (var i = 0; i < playerSelections.Count; i++)
        {
            var characterIndex = playerSelections[i].selectedEntry;
            var character = roster.FighterRoster[characterIndex];
            serviceContainer.PlayerInfoService.AssignFighterToPlayer(i, character);
        }

        // Start the game
        serviceContainer.GameDirector.BeginGame();
    }

    private void HandleChangeCharacter(int playerIndex, int direction)
    {
        var currentIndex = playerSelections[playerIndex].selectedEntry;
        var currentSelection = characterEntriesUI[currentIndex];
        var nextIndex = currentIndex + direction;

        // Loop around
        if (nextIndex < 0)
        {
            nextIndex = characterEntriesUI.Count - 1;
        }
        else if (nextIndex >= characterEntriesUI.Count)
        {
            nextIndex = 0;
        }

        var nextSelection = characterEntriesUI[nextIndex];

        if (currentSelection != null)
        {
            currentSelection.UnselectedBy(playerIndex);
        }

        nextSelection.SelectedBy(playerIndex);

        playerSelections[playerIndex].selectedEntry = nextIndex;
    }

    private void OnDestroy()
    {
        for (var i = 0; i < playerSelections.Count; i++)
        {
            playerSelections[i].Cleanup();
        }
    }

    private class PlayerCharacterSelector
    {
        public int selectedEntry;
        public bool isReady = false;

        private InputProvider inputProvider;
        private int playerIndex;
        private PlayerSelectStatusUI _selectStatusUI;

        public event Action<int> ReadyUp;
        public event Action<int, int> ChangeCharacter;

        public PlayerCharacterSelector(int playerIndex, InputProvider inputProvider,
            PlayerSelectStatusUI selectStatusUI)
        {
            this.inputProvider = inputProvider;
            this.playerIndex = playerIndex;
            _selectStatusUI = selectStatusUI;

            selectStatusUI.SetReadyStatus(false);
            inputProvider.OnJumpInput += HandleJumpInput;
            inputProvider.OnMovementInput += HandleMovementInput;
        }

        private void HandleMovementInput(InputAction.CallbackContext obj)
        {
            if (!obj.started)
            {
                return;
            }

            var value = obj.ReadValue<Vector2>().x;
            if (value > 0.5f)
            {
                ChangeCharacter?.Invoke(playerIndex, 1);
            }
            else if (value < -0.5f)
            {
                ChangeCharacter?.Invoke(playerIndex, -1);
            }
        }

        private void HandleJumpInput(InputAction.CallbackContext obj)
        {
            if (!obj.started)
            {
                return;
            }

            isReady = !isReady;
            _selectStatusUI.SetReadyStatus(isReady);
            ReadyUp?.Invoke(playerIndex);
        }

        public void Cleanup()
        {
            inputProvider.OnJumpInput -= HandleJumpInput;
            inputProvider.OnMovementInput -= HandleMovementInput;
        }
    }
}