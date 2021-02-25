using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerBoardWidget : MonoBehaviour, IPointerClickHandler
    {
        private PlayerData _playerData;
        
        public Text PlayerName;
        public Text Score;
        public GameObject CurrentBorder;
        public Image FilesLoadingStrip;

        public void Bind(PlayerData playerData, bool isCurrent)
        {
            _playerData = playerData;
            PlayerName.text = playerData.Name;
            Score.text = playerData.Score.ToString();
            CurrentBorder.SetActive(isCurrent);
            FilesLoadingStrip.fillAmount = playerData.FilesLoadingPercentage * 1f / 100f;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            MetagameEvents.PlayerBoardWidgetClicked.Publish(_playerData);
        }
    }
}