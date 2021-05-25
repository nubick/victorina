using System;
using UnityEngine.EventSystems;

namespace Victorina
{
    public class CrafterThemeDragItem : DragItemBase
    {
        public CrafterThemeLineWidget ThemeWidget;

        public override void OnDrop(PointerEventData eventData)
        {
            DragItemBase dragItem = eventData.pointerDrag.GetComponent<DragItemBase>();

            if (dragItem == null)
                throw new Exception($"Not supported drag item which was dropped on Theme Drag Item: {eventData.pointerDrag.name}");

            if (dragItem is CrafterQuestionDragItem questionDragItem)
                MetagameEvents.CrafterQuestionDropOnTheme.Publish(questionDragItem.QuestionWidget.Question, ThemeWidget.Theme);
            else if (dragItem is CrafterThemeDragItem themeDragItem)
                MetagameEvents.CrafterThemeDropOnTheme.Publish(themeDragItem.ThemeWidget.Theme, ThemeWidget.Theme);
        }
    }
}