using UnityEngine;
using UnityEngine.EventSystems;

namespace Victorina
{
    public class CrafterThemeButton : MonoBehaviour, IPointerClickHandler
    {
        private Theme _theme;
        
        public void Bind(Theme theme)
        {
            _theme = theme;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 1)
                MetagameEvents.CrafterThemeClicked.Publish(_theme);
            else if (eventData.clickCount == 2)
                MetagameEvents.CrafterThemeNameEditRequested.Publish(_theme);
        }
    }
}