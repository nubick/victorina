using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterRoundTabWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
    {
        private Round _round;
        
        public Text RoundName;
        public GameObject SelectedState;
        public GameObject DeleteButton;
        
        public void Bind(Round round, bool isSelected)
        {
            _round = round;
            RoundName.text = _round.Name;
            SelectedState.SetActive(isSelected);
        }

        public void OnClicked()
        {
            MetagameEvents.CrafterRoundClicked.Publish(_round);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            DeleteButton.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DeleteButton.SetActive(false);
        }

        public void OnDeleteButtonClicked()
        {
            MetagameEvents.CrafterRoundDeleteButtonClicked.Publish(_round);
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log($"Drop: On Round");
            CrafterQuestionDragItem droppedItem = eventData.pointerDrag.GetComponent<CrafterQuestionDragItem>();
            MetagameEvents.CrafterQuestionDropOnRound.Publish(droppedItem.QuestionWidget.Question, _round);
        }
    }
}