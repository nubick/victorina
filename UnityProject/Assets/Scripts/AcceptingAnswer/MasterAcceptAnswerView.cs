using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class MasterAcceptAnswerView : ViewBase
    {
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private MasterAnswerTipData MasterAnswerTipData { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        
        public Text Header;
        public Text AnswerTip;
        
        private AcceptingAnswerPlayState PlayState => PlayStateData.As<AcceptingAnswerPlayState>();

        protected override void OnShown()
        {
            Header.text = $"Отвечает: {PlayersBoardSystem.GetPlayerName(PlayState.AnsweringPlayerId)}";
            AnswerTip.text = $"Ответ: \n{MasterAnswerTipData.AnswerTip}";
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