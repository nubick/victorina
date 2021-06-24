using System;
using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class TextStoryDotView : ViewBase
    {
        [Inject] private PackagePlayStateData PlayStateData { get; set; }

        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();
        
        public Text QuestionText;

        protected override void OnShown()
        {
            if (PlayState.CurrentStoryDot is TextStoryDot textDot)
                QuestionText.text = textDot.Text;
            else
                throw new Exception($"TextQuestionView: Current story dot is not text, {PlayState.CurrentStoryDot}");
        }
    }
}