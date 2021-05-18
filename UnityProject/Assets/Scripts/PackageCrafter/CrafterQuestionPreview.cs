using System.IO;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterQuestionPreview : ViewBase
    {
        [Inject] private CrafterData CrafterData { get; set; }
        
        public Text TextPreviewSmall;
        public Image ImagePreviewSmall;
        public Image ImagePreviewBig;
        public RectTransform StoryDotRoot;
        public StoryDotPreviewWidget StoryDotPreviewWidgetPrefab;

        public void Initialize()
        {
            MetagameEvents.CrafterQuestionSelected.Subscribe(OnQuestionSelected);
            MetagameEvents.CrafterStoryDotPreviewIndexChanged.Subscribe(OnStoryDotIndexChanged);
        }

        private void OnStoryDotIndexChanged(int newIndex)
        {
            if (CrafterData.PreviewStoryDotIndex != newIndex)
            {
                CrafterData.PreviewStoryDotIndex = newIndex;
                RefreshSmallPreview(CrafterData.SelectedQuestion, CrafterData.PreviewStoryDotIndex);
            }
        }
        
        private void OnQuestionSelected(Question question)
        {
            CrafterData.PreviewStoryDotIndex = 0;
            
            if(!IsActive && question != null)
                Show();

            if (IsActive && question == null)
            {
                Hide();
                return;
            }
            
            RefreshStoryDots(question);
            RefreshSmallPreview(question, CrafterData.PreviewStoryDotIndex);
        }

        private void RefreshSmallPreview(Question question, int index)
        {
            StoryDot storyDot = question.GetAllMainStories()[index];
            
            ImagePreviewSmall.gameObject.SetActive(false);
            ImagePreviewSmall.sprite = null;
            ImagePreviewBig.gameObject.SetActive(false);
            ImagePreviewBig.sprite = null;
            
            TextPreviewSmall.gameObject.SetActive(false);

            if (storyDot is TextStoryDot textStoryDot)
            {
                TextPreviewSmall.gameObject.SetActive(true);
                TextPreviewSmall.text = textStoryDot.Text;
            }
            else if (storyDot is ImageStoryDot imageStoryDot)
            {
                ImagePreviewSmall.gameObject.SetActive(true);
                
                byte[] bytes = File.ReadAllBytes(imageStoryDot.Path);
                
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                
                ImagePreviewSmall.sprite = sprite;
                ImagePreviewBig.sprite = sprite;
            }
        }

        private void RefreshStoryDots(Question question)
        {
            ClearChild(StoryDotRoot);

            int index = 0;
            foreach (StoryDot storyDot in question.QuestionStory)
            {
                StoryDotPreviewWidget previewWidget = Instantiate(StoryDotPreviewWidgetPrefab, StoryDotRoot);
                previewWidget.Bind(storyDot, isQuestion: true, index);
                index++;
            }

            foreach (StoryDot storyDot in question.AnswerStory)
            {
                StoryDotPreviewWidget previewWidget = Instantiate(StoryDotPreviewWidgetPrefab, StoryDotRoot);
                previewWidget.Bind(storyDot, isQuestion: false, index);
                index++;
            }
        }
    }
}