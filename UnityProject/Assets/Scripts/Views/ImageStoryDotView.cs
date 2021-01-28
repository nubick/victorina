using System;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class ImageStoryDotView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }

        public Sprite NoImageSprite;
        public Image ImageBorder;
        public Image Image;

        protected override void OnShown()
        {
            if (MatchData.CurrentStoryDot is ImageStoryDot imageDot)
            {
                Image.sprite = GetSprite(imageDot);
                ImageBorder.sprite = Image.sprite;
            }
            else
            {
                throw new Exception($"ImageQuestionView: Current story dot is not image, {MatchData.CurrentStoryDot}");
            }
        }

        private Sprite GetSprite(ImageStoryDot imageStoryDot)
        {
            Sprite sprite = NoImageSprite;

            if (MasterFilesRepository.Has(imageStoryDot.FileId))
            {
                byte[] bytes = MasterFilesRepository.GetBytes(imageStoryDot.FileId);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogWarning($"Don't have loaded file, fileId '{imageStoryDot.FileId}'");
            }
            return sprite;
        }

        public void OnAnswerButtonClicked()
        {
            MatchSystem.ShowNext();
        }
    }
}