using UnityEngine;
using UnityEngine.EventSystems;

namespace Victorina
{
    public class StoryDotPreviewWidget : MonoBehaviour, IPointerEnterHandler
    {
        private int _index;
        
        public GameObject QuestionBackground;
        public GameObject AnswerBackground;
        public GameObject TextIcon;
        public GameObject ImageIcon;
        public GameObject AudioIcon;
        public GameObject VideoIcon;

        public void Bind(StoryDot storyDot, bool isQuestion, int index)
        {
            _index = index;
            QuestionBackground.SetActive(isQuestion);
            AnswerBackground.SetActive(!isQuestion);
            TextIcon.SetActive(storyDot is TextStoryDot);
            ImageIcon.SetActive(storyDot is ImageStoryDot);
            AudioIcon.SetActive(storyDot is AudioStoryDot);
            VideoIcon.SetActive(storyDot is VideoStoryDot);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MetagameEvents.CrafterStoryDotPreviewIndexChanged.Publish(_index);
        }
    }
}