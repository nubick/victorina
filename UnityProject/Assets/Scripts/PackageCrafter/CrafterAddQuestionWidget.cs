using UnityEngine;
using UnityEngine.EventSystems;

namespace Victorina
{
    public class CrafterAddQuestionWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
    {
        private Theme _theme;

        public GameObject VisiblePart;
        
        public void Bind(Theme theme)
        {
            _theme = theme;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            VisiblePart.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            VisiblePart.SetActive(false);
        }

        public void OnDrop(PointerEventData eventData)
        {
            CrafterQuestionDragItem droppedItem = eventData.pointerDrag.GetComponent<CrafterQuestionDragItem>();
            MetagameEvents.CrafterQuestionDropOnTheme.Publish(droppedItem.QuestionWidget.Question, _theme);
        }
    }
}