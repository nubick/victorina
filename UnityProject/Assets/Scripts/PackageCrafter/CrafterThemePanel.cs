using UnityEngine;
using UnityEngine.EventSystems;

namespace Victorina
{
    public class CrafterThemePanel : MonoBehaviour, IPointerClickHandler
    {
        public CrafterThemeLineWidget ThemeLineWidget;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 1)
                MetagameEvents.CrafterThemeClicked.Publish(ThemeLineWidget.Theme);
            else if (eventData.clickCount == 2)
                MetagameEvents.CrafterThemeNameEditRequested.Publish(ThemeLineWidget.Theme);
        }
    }
}