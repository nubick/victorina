using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerMoreInfoWidget : MonoBehaviour, IPointerClickHandler
    {
        private PlayerData _player;
        
        public Text InfoText;
        public GameObject Highlight;
        public GameObject Selection;
        
        public void Bind(PlayerData player, string infoText, bool isHighlighted, bool isSelected)
        {
            _player = player;
            InfoText.text = infoText;
            Highlight.SetActive(isHighlighted);
            Selection.SetActive(isSelected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MetagameEvents.PlayerMoreInfoClicked.Publish(_player);
        }
    }
}