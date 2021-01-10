using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class TextQuestionView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private RoundView RoundView { get; set; }
        
        public Text QuestionText;

        protected override void OnShown()
        {
            QuestionText.text = MatchData.SelectedQuestion.Text;
        }

        public void OnAnswerButtonClicked()
        {
            MatchSystem.BackToRound();
            SwitchTo(RoundView);
        }
    }
}