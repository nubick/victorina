using UnityEngine;
using UnityEngine.EventSystems;

namespace Victorina
{
    public abstract class DragItemBase : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {
        public RectTransform Container;
        public GameObject StartDragArea;
        public GameObject Image;

        public void OnBeginDrag(PointerEventData eventData)
        {
            Image.SetActive(true);
            StartDragArea.SetActive(false);
            MetagameEvents.CrafterBeginDrag.Publish(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Image.SetActive(false);
            StartDragArea.SetActive(true);
            MetagameEvents.CrafterEndDrag.Publish(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        public abstract void OnDrop(PointerEventData eventData);
    }
}