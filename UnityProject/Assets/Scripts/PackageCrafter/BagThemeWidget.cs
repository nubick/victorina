using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class BagThemeWidget : MonoBehaviour, IPointerClickHandler
    {
        private Theme _theme;
        
        public GameObject DefaultBackground;
        public GameObject SelectedBackground;
        public Text Name;

        public void Bind(Theme theme, bool isSelected)
        {
            _theme = theme;
            Name.text = theme.Name;
            DefaultBackground.SetActive(!isSelected);
            SelectedBackground.SetActive(isSelected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MetagameEvents.CrafterBagThemeClicked.Publish(_theme);   
        }
    }
}