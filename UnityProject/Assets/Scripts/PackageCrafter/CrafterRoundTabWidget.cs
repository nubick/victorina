using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterRoundTabWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IPointerClickHandler
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

        public void Select()
        {
            SelectedState.SetActive(true);
        }

        public void UnSelect()
        {
            SelectedState.SetActive(false);
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
            DragItemBase dragItem = eventData.pointerDrag.GetComponent<DragItemBase>();

            if (dragItem == null)
                throw new Exception($"Not supported drag item which was dropped on Round Tab Widget: {eventData.pointerDrag.name}");

            if (dragItem is CrafterQuestionDragItem questionDragItem)
                MetagameEvents.CrafterQuestionDropOnRound.Publish(questionDragItem.QuestionWidget.Question, _round);
            else if (dragItem is CrafterThemeDragItem themeDragItem)
                MetagameEvents.CrafterThemeDropOnRound.Publish(themeDragItem.ThemeWidget.Theme, _round);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 1)
                MetagameEvents.CrafterRoundClicked.Publish(_round);
            else if (eventData.clickCount == 2)
                MetagameEvents.CrafterRoundNameEditRequested.Publish(_round);
        }
    }
}