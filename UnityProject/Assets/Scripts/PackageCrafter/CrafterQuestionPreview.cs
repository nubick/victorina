using System.IO;
using Injection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Victorina
{
    public class CrafterQuestionPreview : ViewBase, IKeyPressedHandler
    {
        [Inject] private CrafterData CrafterData { get; set; }
        
        public Text TextPreviewSmall;
        
        public Image ImagePreviewSmall;
        public Image ImagePreviewBig;

        public GameObject VideoPreviewSmall;
        public GameObject VideoPreviewBig;
        public VideoPlayer VideoPlayer;

        public GameObject AudioPreviewSmall;
        public AudioSource AudioSource;
        
        public RectTransform StoryDotRoot;
        public StoryDotPreviewWidget StoryDotPreviewWidgetPrefab;
        public GameObject BigPreviewsContainer;
        
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
            StoryDot storyDot = question.GetAllStories()[index];
            
            HideBigPreview();
            
            ImagePreviewSmall.gameObject.SetActive(false);
            ImagePreviewSmall.sprite = null;
            ImagePreviewBig.gameObject.SetActive(false);
            ImagePreviewBig.sprite = null;
            
            VideoPreviewSmall.SetActive(false);
            VideoPreviewBig.SetActive(false);
            VideoPlayer.Stop();

            AudioPreviewSmall.SetActive(false);
            AudioSource.Stop();
            
            TextPreviewSmall.gameObject.SetActive(false);

            if (storyDot is TextStoryDot textStoryDot)
            {
                TextPreviewSmall.gameObject.SetActive(true);
                TextPreviewSmall.text = textStoryDot.Text;
            }
            else if (storyDot is ImageStoryDot imageStoryDot)
            {
                ImagePreviewSmall.gameObject.SetActive(true);
                ImagePreviewBig.gameObject.SetActive(true);
                
                byte[] bytes = File.ReadAllBytes(imageStoryDot.Path);
                Debug.Log($"Preview: {imageStoryDot.Path}, size: {bytes.Length/1024}kb");

                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                
                ImagePreviewSmall.sprite = sprite;
                ImagePreviewBig.sprite = sprite;
            }
            else if (storyDot is VideoStoryDot videoStoryDot)
            {
                VideoPreviewSmall.SetActive(true);
                VideoPreviewBig.SetActive(true);
                
                string requestPath = $"file://{videoStoryDot.Path}";
                VideoPlayer.url = requestPath;
                VideoPlayer.Prepare();
                VideoPlayer.Play();
            }
            else if (storyDot is AudioStoryDot audioStoryDot)
            {
                AudioPreviewSmall.SetActive(true);
                StartCoroutine(AudioStoryDotView.PlayAudioByPath(AudioSource, audioStoryDot.Path));
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

        public void OnKeyPressed(KeyCode keyCode)
        {
            if (!IsActive || CrafterData.SelectedQuestion == null)
                return;
            
            if (keyCode == KeyCode.LeftArrow)
            {
                if (CrafterData.PreviewStoryDotIndex > 0)
                    OnStoryDotIndexChanged(CrafterData.PreviewStoryDotIndex - 1);
            }
            else if (keyCode == KeyCode.RightArrow)
            {
                int amount = CrafterData.SelectedQuestion.GetAllStories().Count;
                if (CrafterData.PreviewStoryDotIndex < amount - 1)
                    OnStoryDotIndexChanged(CrafterData.PreviewStoryDotIndex + 1);
            }
            else if (keyCode == KeyCode.UpArrow)
            {
                ShowBigPreview();
            }
            else if (keyCode == KeyCode.DownArrow)
            {
                HideBigPreview();
            }
        }

        public void ShowBigPreview()
        {
            BigPreviewsContainer.SetActive(true);
        }

        public void HideBigPreview()
        {
            BigPreviewsContainer.SetActive(false);
        }
    }
}