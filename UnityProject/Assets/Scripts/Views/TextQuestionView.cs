using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class TextQuestionView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }
        
        public Text QuestionText;

        protected override void OnShown()
        {
            QuestionText.text = MatchData.TextQuestion?.Question;
        }
    }
}