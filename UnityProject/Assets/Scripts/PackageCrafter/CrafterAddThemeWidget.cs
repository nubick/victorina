using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Victorina
{
    public class CrafterAddThemeWidget : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            DragItemBase dragItem = eventData.pointerDrag.GetComponent<DragItemBase>();

            if (dragItem == null)
                throw new Exception($"Not supported drag item which was dropped on Add Theme Widget: {eventData.pointerDrag.name}");

            if (dragItem is CrafterThemeDragItem themeDragItem)
                MetagameEvents.CrafterThemeDropOnTheme.Publish(themeDragItem.ThemeWidget.Theme, null);
        }
    }
}