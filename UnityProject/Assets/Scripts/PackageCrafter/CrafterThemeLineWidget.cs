using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterThemeLineWidget : MonoBehaviour
    {
        private Theme _theme;
        
        public Text ThemeName;
        public GameObject DefaultBackground;
        public GameObject SelectedBackground;
        public Transform QuestionsRoot;
        public CrafterThemeButton ThemeButton;
        
        public void Bind(Theme theme, bool isSelected)
        {
            _theme = theme;
            ThemeName.text = theme.Name;
            SetSelectedState(isSelected);
            ThemeButton.Bind(_theme);
        }

        public void Select()
        {
            SetSelectedState(isSelected: true);
        }

        public void UnSelect()
        {
            SetSelectedState(isSelected: false);
        }
        
        private void SetSelectedState(bool isSelected)
        {
            DefaultBackground.SetActive(!isSelected);
            SelectedBackground.SetActive(isSelected);
        }
        
        public void OnThemeDeleteButtonClicked()
        {
            MetagameEvents.CrafterThemeDeleteButtonClicked.Publish(_theme);
        }
    }
}