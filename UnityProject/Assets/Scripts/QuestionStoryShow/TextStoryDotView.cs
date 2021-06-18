using System;
using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class TextStoryDotView : ViewBase
    {
        [Inject] private QuestionStoryShowData Data { get; set; }

        public Text QuestionText;

        protected override void OnShown()
        {
            if (Data.CurrentStoryDot is TextStoryDot textDot)
                QuestionText.text = textDot.Text;
            else
                throw new Exception($"TextQuestionView: Current story dot is not text, {Data.CurrentStoryDot}");
        }
    }
}