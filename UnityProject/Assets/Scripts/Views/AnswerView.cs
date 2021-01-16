using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class AnswerView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        
        public Text Answer;

        protected override void OnShown()
        {
            Answer.text = MatchData.SelectedQuestion.Answer;
        }

        public void OnNextButtonClicked()
        {
            MatchSystem.BackToRound();
        }
    }
}