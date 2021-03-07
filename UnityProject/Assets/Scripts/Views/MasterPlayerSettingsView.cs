using Injection;
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
        
        public void Bind(PlayerData playerData)
        {
            _playerData = playerData;
            PlayerId.text = $"ID игрока: {playerData.Id}";
            PlayerNameInputField.Text = playerData.Name;
        }
        
        public void OnMakeCurrentButtonClicked()
        {
            PlayersBoardSystem.MakePlayerCurrent(_playerData);
            Hide();
        }

        public void OnUpdateButtonClicked()
        {
            if (_playerData.Name != PlayerNameInputField.Text)
            {
                string newPlayerName = PlayerNameInputField.Text;
                if (ServerService.IsPlayerNameValid(newPlayerName))
                {
                    PlayersBoardSystem.UpdatePlayerName(_playerData, newPlayerName);
                    Hide();
                }
                else
                {
                    PlayerNameInputField.MarkInvalid();
                }
            }
        }
        
        public void OnCancelButtonClicked()
        {
            Hide();
        }
    }
}