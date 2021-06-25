using System;
using UnityEngine.UI;

namespace Victorina
{
    public class TextStoryDotView : StoryDotView
    {
        public Text QuestionText;

        protected override void OnShown()
        {
            StoryDot currentStoryDot = GetCurrentStoryDot();
            if (currentStoryDot is TextStoryDot textDot)
                QuestionText.text = textDot.Text;
            else
                throw new Exception($"TextQuestionView: Current story dot is not text, {currentStoryDot}");
        }
    }
}