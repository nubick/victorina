using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterQuestionWidget : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Question _question;
        
        public Text Price;
        public GameObject DefaultBackground;
        public GameObject SelectedBackground;
        public GameObject DeleteButton;

        public void Bind(Question question, bool isSelected)
        {
            _question = question;
            Price.text = question.Price.ToString();
            DefaultBackground.SetActive(!isSelected);
            SelectedBackground.SetActive(isSelected);
            DeleteButton.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MetagameEvents.CrafterQuestionClicked.Publish(_question);
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
            MetagameEvents.CrafterQuestionDeleteButtonClicked.Publish(_question);
        }
    }
}