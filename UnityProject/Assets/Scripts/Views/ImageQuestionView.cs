using System;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class ImageQuestionView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }

        public Image ImageBorder;
        public Image Image;

        protected override void OnShown()
        {
            if (MatchData.CurrentStoryDot is ImageStoryDot imageDot)
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(imageDot.Bytes);
                Image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                ImageBorder.sprite = Image.sprite;
            }
            else
            {
                throw new Exception($"ImageQuestionView: Current story dot is not image, {MatchData.CurrentStoryDot}");
            }
        }
        
        public void OnAnswerButtonClicked()
        {
            MatchSystem.ShowNext();
        }
    }
}