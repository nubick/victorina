using System.Text;
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
        public Text StoriesInfo;

        public GameObject CatInBagIcon;
        public GameObject AuctionIcon;
        public GameObject NoRiskIcon;
        
        public void Bind(Question question, bool isSelected)
        {
            _question = question;
            Price.text = question.Price.ToString();
            DefaultBackground.SetActive(!isSelected);
            SelectedBackground.SetActive(isSelected);
            DeleteButton.SetActive(false);
            CatInBagIcon.SetActive(question.Type == QuestionType.CatInBag);
            AuctionIcon.SetActive(question.Type == QuestionType.Auction);
            NoRiskIcon.SetActive(question.Type == QuestionType.NoRisk);
            FillStoriesInfo(_question);
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