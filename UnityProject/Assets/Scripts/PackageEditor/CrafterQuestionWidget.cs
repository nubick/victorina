using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterQuestionWidget : MonoBehaviour, IPointerClickHandler
    {
        private Question _question;
        
        public Text Price;
        public GameObject DefaultBackground;
        public GameObject SelectedBackground;

        public void Bind(Question question, bool isSelected)
        {
            _question = question;
            Price.text = question.Price.ToString();
            DefaultBackground.SetActive(!isSelected);
            SelectedBackground.SetActive(isSelected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MetagameEvents.CrafterQuestionClicked.Publish(_question);
        }
    }
}