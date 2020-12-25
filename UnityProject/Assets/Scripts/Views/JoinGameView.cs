using Injection;
using UnityEngine;

namespace Victorina
{
    public class JoinGameView : ViewBase
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private ClientService ClientService { get; set; }
        [Inject] private GameLobbyView GameLobbyView { get; set; }
        [Inject] private IpCodeSystem IpCodeSystem { get; set; }
        [Inject] private AppState AppState { get; set; }
        [Inject] private SaveSystem SaveSystem { get; set; }

        public ValidatedInputField PlayerNameInputField;
        public ValidatedInputField GameCodeInputField;
        public GameObject UseLocalhostButton;

        protected override void OnShown()
        {
            if (string.IsNullOrWhiteSpace(PlayerNameInputField.Text))
                PlayerNameInputField.Text = AppState.LastJoinPlayerName;

            if (string.IsNullOrWhiteSpace(GameCodeInputField.Text))
                GameCodeInputField.Text = AppState.LastJoinGameCode;
            
            UseLocalhostButton.SetActive(true);//todo: link to build mode settings
        }

        public void OnJoinButtonClicked()
        {
            bool hasValidationError = false;
            
            string playerName = PlayerNameInputField.Text;
            if (!IsPlayerNameValid(playerName))
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
                ClientService.JoinGame(playerName, gameCode);
                
                //todo: go to lobby only after success join

                AppState.LastJoinPlayerName = playerName;
                AppState.LastJoinGameCode = gameCode;
                SaveSystem.Save();
                
                SwitchTo(GameLobbyView);
            }
        }

        private bool IsPlayerNameValid(string playerName)
        {
            return !string.IsNullOrWhiteSpace(playerName);
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