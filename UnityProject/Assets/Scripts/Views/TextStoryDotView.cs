using System;
using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class TextStoryDotView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }
        
        public Text QuestionText;

        protected override void OnShown()
        {
            if (MatchData.QuestionAnswerData.CurrentStoryDot is TextStoryDot textDot)
                QuestionText.text = textDot.Text;
            else
                throw new Exception($"TextQuestionView: Current story dot is not text, {MatchData.QuestionAnswerData.CurrentStoryDot}");
        }
    }
}