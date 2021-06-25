using System;
using System.Linq;
using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class QuestionAnswerSystem
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private MasterShowQuestionView MasterShowQuestionView { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private DataChangeHandler DataChangeHandler { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public bool CanBackToRound()
        {
            //todo: finish refactoring
            return false;
            //return Data.Phase.Value == QuestionPhase.ShowAnswer && Data.IsLastDot;
        }
        
        public void BackToRound()
        {
            CommandsSystem.AddNewCommand(new FinishQuestionCommand());
        }
        
        public void CancelAcceptingAnswer()
        {
            CommandsSystem.AddNewCommand(new CancelAcceptingAnswerCommand());
        }

        public void AcceptAnswerAsCorrect()
        {
            CommandsSystem.AddNewCommand(new AcceptAnswerAsCorrectCommand());
        }

        public void AcceptAnswerAsWrong()
        {
           CommandsSystem.AddNewCommand(new AcceptAnswerAsWrongCommand());
        }
    }
}