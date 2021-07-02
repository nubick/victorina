using Commands;
using Injection;
using UnityEngine;
using UnityEngine.UI;
using Victorina.Commands;
using Victorina.DevTools;

namespace Victorina
{
    public class MasterPlayerSettingsView : ViewBase
    {
        private PlayerData _playerData;
        
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private DevToolsSystem DevToolsSystem { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public Text PlayerId;
        public ValidatedInputField PlayerNameInputField;
        public ValidatedInputField PlayerScoreInputField;

        public GameObject DevToolsPanel;
        
        public void Show(PlayerData playerData)
        {
            _playerData = playerData;
            DevToolsPanel.SetActive(Static.BuildMode == BuildMode.Development);
            Show();
        }
        
        protected override void OnShown()
        {
            PlayerId.text = $"ID игрока: {_playerData.PlayerId}";
            PlayerNameInputField.Text = _playerData.Name;
            PlayerScoreInputField.Text = _playerData.Score.ToString();
        }
        
        public void OnMakeCurrentButtonClicked()
        {
            CommandsSystem.AddNewCommand(new MasterMakePlayerAsCurrentCommand(_playerData.PlayerId));
            Hide();
        }

        public void OnUpdateButtonClicked()
        {
            UpdatePlayerName();
            UpdatePlayerScore();
            Hide();
        }

        private void UpdatePlayerName()
        {
            if (_playerData.Name != PlayerNameInputField.Text)
            {
                string newPlayerName = PlayerNameInputField.Text;
                if (PlayersBoardSystem.IsPlayerNameValid(newPlayerName))
                    CommandsSystem.AddNewCommand(new MasterUpdatePlayerNameCommand(_playerData.PlayerId, newPlayerName));
                else
                    PlayerNameInputField.MarkInvalid();
            }
        }

        private void UpdatePlayerScore()
        {
            if (int.TryParse(PlayerScoreInputField.Text, out int newScore))
            {
                CommandsSystem.AddNewCommand(new MasterUpdatePlayerScoreCommand(_playerData.PlayerId, newScore));
            }
            else
            {
                Debug.LogWarning($"Can't parse player score: '{PlayerScoreInputField.Text}'");
                PlayerScoreInputField.MarkInvalid();
            }
        }

        public void OnChangeScoreButtonClicked(int change)
        {
            if (!int.TryParse(PlayerScoreInputField.Text, out int score))
                score = _playerData.Score;

            score += change;
            PlayerScoreInputField.Text = score.ToString();
        }

        public void OnCancelButtonClicked()
        {
            Hide();
        }

        public void OnRequestPlayerLogsButtonClicked()
        {
            DevToolsSystem.RequestPlayerLogs(_playerData);
        }
    }
}