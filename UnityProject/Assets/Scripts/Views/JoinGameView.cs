using System.Collections;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class JoinGameView : ViewBase
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private ClientService ClientService { get; set; }
        [Inject] private IpCodeSystem IpCodeSystem { get; set; }
        [Inject] private AppState AppState { get; set; }
        [Inject] private SaveSystem SaveSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private ServerService ServerService { get; set; }

        public ValidatedInputField PlayerNameInputField;
        public ValidatedInputField GameCodeInputField;
        public GameObject UseLocalhostButton;

        public CanvasGroup CanvasGroup;
        public GameObject LoadingIndicator;
        
        protected override void OnShown()
        {
            CanvasGroup.interactable = true;
            LoadingIndicator.SetActive(false);
            
            if (string.IsNullOrWhiteSpace(PlayerNameInputField.Text))
                PlayerNameInputField.Text = AppState.LastJoinPlayerName;

            if (string.IsNullOrWhiteSpace(GameCodeInputField.Text))
                GameCodeInputField.Text = AppState.LastJoinGameCode;
            
            UseLocalhostButton.SetActive(Static.BuildMode == BuildMode.Development);
        }

        public void OnJoinButtonClicked()
        {
            bool hasValidationError = false;
            
            string playerName = PlayerNameInputField.Text;
            if (!ServerService.IsPlayerNameValid(playerName))
            {
                PlayerNameInputField.MarkInvalid();
                hasValidationError = true;
            }
            
            string gameCode = GameCodeInputField.Text.ToUpper();
            if (!IsGameCodeValid(gameCode))
            {
                Debug.Log($"Game code: '{gameCode}' is invalid");
                GameCodeInputField.MarkInvalid();
                hasValidationError = true;
            }
            
            if (!hasValidationError)
            {
                StartCoroutine(JoinCoroutine(playerName, gameCode));
            }
        }

        private IEnumerator JoinCoroutine(string playerName, string gameCode)
        {
            CanvasGroup.interactable = false;
            LoadingIndicator.SetActive(true);
            
            yield return ClientService.JoinGame(playerName, gameCode);

            CanvasGroup.interactable = true;
            LoadingIndicator.SetActive(false);
            
            if (NetworkData.ClientConnectingState == ClientConnectingState.Success)
            {
                AppState.LastJoinPlayerName = playerName;
                AppState.LastJoinGameCode = gameCode;
                SaveSystem.Save();
            }
            else
            {
                Debug.Log($"Join game error, client connecting state: {NetworkData.ClientConnectingState}");
            }
        }
        
        private bool IsGameCodeValid(string gameCode)
        {
            return IpCodeSystem.IsValidGameCode(gameCode);
        }
        
        public void OnUseLocalhostButtonClicked()
        {
            GameCodeInputField.Text = Static.LocalhostGameCode;
        }

        public void OnBackButtonClicked()
        {
            SwitchTo(StartupView);
        }
    }
}