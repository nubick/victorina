using UnityEngine;
using UnityEngine.EventSystems;

namespace Victorina
{
    public class CrafterQuestionDragItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {
        public CrafterQuestionWidget QuestionWidget;
        public RectTransform Container;
        public GameObject StartDragArea;
        public GameObject Image;
        
        private RectTransform RectTransform => transform as RectTransform;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            Image.SetActive(true);
            StartDragArea.SetActive(false);
            MetagameEvents.CrafterQuestionBeginDrag.Publish(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Image.SetActive(false);
            StartDragArea.SetActive(true);
            MetagameEvents.CrafterQuestionEndDrag.Publish(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        public void OnDrop(PointerEventData eventData)
        {
            CrafterQuestionDragItem droppedItem = eventData.pointerDrag.GetComponent<CrafterQuestionDragItem>();
            MetagameEvents.CrafterQuestionDropOnQuestion.Publish(droppedItem, this);
        }
    }
}