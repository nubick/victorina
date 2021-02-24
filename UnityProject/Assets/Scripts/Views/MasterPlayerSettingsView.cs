using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class MasterPlayerSettingsView : ViewBase
    {
        private PlayerData _playerData;
        
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        
        public Text PlayerId;
        public Text PlayerName;
        
        public void Bind(PlayerData playerData)
        {
            _playerData = playerData;
            PlayerId.text = $"ID игрока: {playerData.Id}";
            PlayerName.text = $"Имя: {playerData.Name}";
        }
        
        public void OnMakeCurrentButtonClicked()
        {
            PlayersBoardSystem.MakePlayerCurrent(_playerData);
            Hide();
        }

        public void OnCancelButtonClicked()
        {
            Hide();
        }
    }
}