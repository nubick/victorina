using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Victorina
{
    public class CrafterQuestionDragItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {
        public CrafterQuestionWidget QuestionWidget;
        public GameObject StartDragArea;
        public GameObject Image;
        
        private RectTransform RectTransform => transform as RectTransform;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            //Debug.Log($"Begin Drag");
            Image.SetActive(true);
            StartDragArea.SetActive(false);
            MetagameEvents.CrafterQuestionBeginDrag.Publish(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (transform == null)
                return;
            
            Debug.Log("End Drag");
            Image.SetActive(false);
            StartDragArea.SetActive(true);
            RectTransform.SetLeftTopRightBottom(0f, 0f, 0f, 0f);
            MetagameEvents.CrafterQuestionEndDrag.Publish(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        public void OnDrop(PointerEventData eventData)
        {
            CrafterQuestionDragItem droppedItem = eventData.pointerDrag.GetComponent<CrafterQuestionDragItem>();
            Debug.Log($"Q-OnDrop: {QuestionWidget.Question.Price}");
            MetagameEvents.CrafterQuestionDropOnQuestion.Publish(droppedItem, this);
        }
    }
}