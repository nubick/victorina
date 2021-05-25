using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterThemeLineWidget : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Theme Theme { get; private set; }
        
        public Text ThemeName;
        public GameObject DefaultBackground;
        public GameObject SelectedBackground;
        public Transform QuestionsRoot;
        public GameObject HoverState;

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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 1)
                MetagameEvents.CrafterThemeClicked.Publish(Theme);
            else if (eventData.clickCount == 2)
                MetagameEvents.CrafterThemeNameEditRequested.Publish(Theme);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HoverState.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HoverState.SetActive(false);
        }
    }
}