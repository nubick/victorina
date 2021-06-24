using System;
using System.Linq;
using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class QuestionAnswerSystem
    {
        [Inject] private QuestionAnswerData Data { get; set; }
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private MasterShowQuestionView MasterShowQuestionView { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private DataChangeHandler DataChangeHandler { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }

        private bool IsLastQuestionStoryDot() => false; 
            //todo: finish refactoring
            //Data.TimerState == QuestionTimerState.NotStarted //&&
                                                 //Data.Phase.Value == QuestionPhase.ShowQuestion; //&&
                                                 //todo: finish refactoring
                                                 //Data.CurrentStoryDot == Data.SelectedQuestion.Value.QuestionStory.Last();
        
        public void StartAnswer(NetQuestion netQuestion)
        {
            //todo: finish refactoring
            //Data.SelectedQuestion.Value = netQuestion;
            //SendToPlayersService.SendSelectedQuestion(Data.SelectedQuestion.Value);

            QuestionTimer.Reset(Static.TimeForAnswer);

            Data.TimerState = QuestionTimerState.NotStarted;
            Data.WrongAnsweredIds.Clear();
            ResetAdmittedPlayersIds(netQuestion.Type);
            
            //Data.AnswerTip = GetAnswerTip(Data.SelectedQuestion.Value);
            Data.IsAnswerTipEnabled = false;
            
            if (IsLastQuestionStoryDot())
                StartTimer();
            
            SendData(MasterIntention.StartAnswering);
        }

        private void ResetAdmittedPlayersIds(QuestionType questionType)
        {
            Data.AdmittedPlayersIds.Clear();
            switch (questionType)
            {
                case QuestionType.Simple:
                    Data.AdmittedPlayersIds.AddRange(PlayersBoard.Players.Select(_ => _.PlayerId));
                    break;
                case QuestionType.NoRisk:
                    Data.AdmittedPlayersIds.Add(PlayersBoard.Current.PlayerId);
                    break;
                case QuestionType.CatInBag:
                    //add who will get cat in bag
                    break;
                case QuestionType.Auction:
                    //add who will win auction later
                    break;
                default:
                    throw new Exception($"Not supported question type: {questionType}");
            }
        }
        
        private void SendData(MasterIntention intention)
        {
            Data.MasterIntention = intention;
            SendToPlayersService.Send(Data);
            DataChangeHandler.HandleMasterIntention(Data);
            MasterShowQuestionView.RefreshUI();
        }
        
        public void StartQuestionStory()
        {
            //Data.Phase.Value = QuestionPhase.ShowQuestion;
            //todo: finish refactoring
            //Data.CurrentStoryDotIndex = 0;
            
            if(IsLastQuestionStoryDot())
                StartTimer();
            
            SendData(MasterIntention.ShowStoryDot);
        }
        
        public void ShowNext()
        {
            //todo: finish refactoring
            //Data.CurrentStoryDotIndex++;
            
            if(IsLastQuestionStoryDot())
                StartTimer();
            
            SendData(MasterIntention.ShowStoryDot);
        }
        
        public void ShowPrevious()
        {
            //todo: finish refactoring
            //Data.CurrentStoryDotIndex--;
            SendData(MasterIntention.ShowStoryDot);
        }
        
        private void StartTimer()
        {
            Data.TimerResetSeconds = Static.TimeForAnswer;
            Data.TimerLeftSeconds = QuestionTimer.LeftSeconds;
            Data.TimerState = QuestionTimerState.Running;
            PlayersButtonClickData.Clear();
        }
        
        public void PauseTimer()
        {
            Data.TimerState = QuestionTimerState.Paused;
            SendData(MasterIntention.PauseTimer);
        }

        public void ContinueTimer()
        {
            StartTimer();
            SendData(MasterIntention.ContinueTimer);
        }

        public void RestartMedia()
        {
            QuestionTimer.Reset(Static.TimeForAnswer);
            StartTimer();
            SendData(MasterIntention.RestartMedia);
        }

        public bool CanShowAnswer()
        {
            return false;
            //todo: finish refactoring
            //return Data.SelectedQuestion.Value.Type == QuestionType.Simple && Data.Phase.Value == QuestionPhase.ShowQuestion && Data.TimerState != QuestionTimerState.NotStarted;
        }
        
        public void ShowAnswer()
        {
            CommandsSystem.AddNewCommand(new ShowAnswerCommand());
        }
        
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
        
        #region Accepting answer

        public void SelectFastestPlayerForAnswer()
        {
            if (NetworkData.IsMaster)
                CommandsSystem.AddNewCommand(new SelectFastestPlayerForAnswerCommand());
        }
        
        public void SelectPlayerForAnswer(byte playerId)
        {
            if (NetworkData.IsMaster)
                CommandsSystem.AddNewCommand(new SelectPlayerForAnswerCommand {PlayerId = playerId});
        }

        public void AcceptNoRiskAnswer()
        {
            if (PlayersBoard.Current == null)
            {
                Debug.Log("Master. Error. Can't accept no risk answer. Current player is null.");
            }
            else
            {
                PauseTimer();
                SelectPlayerForAnswer(PlayersBoard.Current.PlayerId);
            }
        }
        
        public string GetAnswerTip(NetQuestion netQuestion)
        {
            StoryDot lastStoryDot = netQuestion.AnswerStory.Last();
            if (lastStoryDot is TextStoryDot textStoryDot)
            {
                return textStoryDot.Text;
            }
            throw new Exception($"Last answer story dot is not text, {lastStoryDot}");
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
        
        #endregion
    }
}