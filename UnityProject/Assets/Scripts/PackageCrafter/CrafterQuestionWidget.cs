using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterQuestionWidget : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Question Question { get; private set; }
        
        public Text Price;
        public GameObject DefaultBackground;
        public GameObject SelectedBackground;
        public Text StoriesInfo;

        public GameObject DeleteButton;
        public GameObject DropPlaceHighlight;
        
        public GameObject CatInBagIcon;
        public GameObject AuctionIcon;
        public GameObject NoRiskIcon;
        
        public void Bind(Question question, bool isSelected)
        {
            Question = question;
            Price.text = question.Price.ToString();
            DefaultBackground.SetActive(!isSelected);
            SelectedBackground.SetActive(isSelected);
            DeleteButton.SetActive(false);
            DropPlaceHighlight.SetActive(false);
            CatInBagIcon.SetActive(question.Type == QuestionType.CatInBag);
            AuctionIcon.SetActive(question.Type == QuestionType.Auction);
            NoRiskIcon.SetActive(question.Type == QuestionType.NoRisk);
            FillStoriesInfo(Question);
        }

        private void FillStoriesInfo(Question question)
        {
            StringBuilder sb = new StringBuilder();

            foreach (StoryDot storyDot in question.QuestionStory)
                sb.Append(storyDot.ToLetter());

            sb.Append("=>");

            foreach (StoryDot storyDot in question.AnswerStory)
                sb.Append(storyDot.ToLetter());

            StoriesInfo.text = sb.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MetagameEvents.CrafterQuestionClicked.Publish(Question);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            DeleteButton.SetActive(true);
            DropPlaceHighlight.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DeleteButton.SetActive(false);
            DropPlaceHighlight.SetActive(false);
        }

        public void OnDeleteButtonClicked()
        {
            MetagameEvents.CrafterQuestionDeleteButtonClicked.Publish(Question);
        }
    }
}