using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterThemeLineWidget : MonoBehaviour, IDropHandler
    {
        private Theme _theme;
        
        public Text ThemeName;
        public GameObject DefaultBackground;
        public GameObject SelectedBackground;
        public Transform QuestionsRoot;
        
        public void Bind(Theme theme, bool isSelected)
        {
            _theme = theme;
            ThemeName.text = theme.Name;
            DefaultBackground.SetActive(!isSelected);
            SelectedBackground.SetActive(isSelected);
        }

        public void OnThemeButtonClicked()
        {
            MetagameEvents.CrafterThemeClicked.Publish(_theme);
        }

        public void OnThemeDeleteButtonClicked()
        {
            MetagameEvents.CrafterThemeDeleteButtonClicked.Publish(_theme);
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log($"OnDrop: {_theme.Name}");
        }
    }
}