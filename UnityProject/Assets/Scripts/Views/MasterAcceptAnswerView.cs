using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class MasterAcceptAnswerView : ViewBase
    {
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        
        public Text Header;
        public Text AnswerTip;

        protected override void OnShown()
        {
            Header.text = $"Отвечает: {QuestionAnswerData.AnsweringPlayerName}";
            AnswerTip.text = $"Ответ: \n{QuestionAnswerData.AnswerTip}";
        }
        
        public void OnCorrectButtonClicked()
        {
            QuestionAnswerSystem.AcceptAnswerAsCorrect();
        }

        public void OnWrongButtonClicked()
        {
            QuestionAnswerSystem.AcceptAnswerAsWrong();
        }

        public void OnCancelButtonClicked()
        {
            QuestionAnswerSystem.CancelAcceptingAnswer();
        }
    }
}