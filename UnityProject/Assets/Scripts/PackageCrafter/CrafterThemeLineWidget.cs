using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterThemeLineWidget : MonoBehaviour
    {
        public Theme Theme { get; private set; }
        
        public Text ThemeName;
        public GameObject DefaultBackground;
        public GameObject SelectedBackground;
        public Transform QuestionsRoot;

        public void Bind(Theme theme, bool isSelected)
        {
            Theme = theme;
            ThemeName.text = theme.Name;
            SetSelectedState(isSelected);
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
            MetagameEvents.CrafterThemeDeleteButtonClicked.Publish(Theme);
        }

        public void OnMoveToBagButtonClicked()
        {
            MetagameEvents.CrafterThemeMoveToBagButtonClicked.Publish(Theme);
        }
    }
}