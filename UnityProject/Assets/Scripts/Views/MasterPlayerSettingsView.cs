using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class MasterPlayerSettingsView : ViewBase
    {
        private PlayerData _playerData;
        
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        
        public Text PlayerId;
        public ValidatedInputField PlayerNameInputField;
        public ValidatedInputField PlayerScoreInputField;
        
        public void Show(PlayerData playerData)
        {
            _playerData = playerData;
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
            PlayersBoardSystem.MakePlayerCurrent(_playerData);
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
                if (ServerService.IsPlayerNameValid(newPlayerName))
                {
                    PlayersBoardSystem.UpdatePlayerName(_playerData, newPlayerName);
                }
                else
                {
                    PlayerNameInputField.MarkInvalid();
                }
            }
        }

        private void UpdatePlayerScore()
        {
            if (int.TryParse(PlayerScoreInputField.Text, out int newScore))
            {
                if (newScore != _playerData.Score)
                {
                    PlayersBoardSystem.UpdatePlayerScore(_playerData, newScore);
                }
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
    }
}