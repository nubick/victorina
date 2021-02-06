using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerButtonClickWidget : MonoBehaviour, IPointerClickHandler
    {
        private PlayerButtonClickData _playerData;
        
        public Text Name;
        public Text Time;

        public void Bind(PlayerButtonClickData playerData)
        {
            _playerData = playerData;
            Name.text = playerData.Name;
            Time.text = $"{playerData.Time:0.0} сек";
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            MetagameEvents.PlayerButtonClickWidgetClicked.Publish(_playerData);
        }
    }
}