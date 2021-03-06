using System;
using UnityEngine.EventSystems;

namespace Victorina
{
    public class CrafterQuestionDragItem : DragItemBase
    {
        public CrafterQuestionWidget QuestionWidget;

        public override void OnDrop(PointerEventData eventData)
        {
            DragItemBase dragItem = eventData.pointerDrag.GetComponent<DragItemBase>();

            if (dragItem == null)
                throw new Exception($"Not supported drag item which was dropped on Theme Drag Item: {eventData.pointerDrag.name}");

            if (dragItem is CrafterQuestionDragItem questionDragItem)
                MetagameEvents.CrafterQuestionDropOnQuestion.Publish(questionDragItem.QuestionWidget.Question, QuestionWidget.Question);
        }
    }
}