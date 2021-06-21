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
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private MasterQuestionPanelView MasterQuestionPanelView { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private DataChangeHandler DataChangeHandler { get; set; }
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }

        private bool IsLastQuestionStoryDot() => false; 
            //todo: finish refactoring
            //Data.TimerState == QuestionTimerState.NotStarted //&&
                                                 //Data.Phase.Value == QuestionPhase.ShowQuestion; //&&
                                                 //todo: finish refactoring
                                                 //Data.CurrentStoryDot == Data.SelectedQuestion.Value.QuestionStory.Last();
        
        public void StartAnswer(NetQuestion netQuestion)
        {
            Data.SelectedQuestion.Value = netQuestion;
            SendToPlayersService.SendSelectedQuestion(Data.SelectedQuestion.Value);

            QuestionTimer.Reset(Static.TimeForAnswer);

            Data.TimerState = QuestionTimerState.NotStarted;
            Data.WrongAnsweredIds.Clear();
            ResetAdmittedPlayersIds(netQuestion.Type);
            //todo: finish refactoring
            //Data.CurrentStoryDotIndex = 0;
            Data.AnsweringPlayerId = 0;
            
            Data.AnswerTip = GetAnswerTip(Data.SelectedQuestion.Value);
            Data.IsAnswerTipEnabled = false;
            
            if (IsLastQuestionStoryDot())
                StartTimer();

            if (netQuestion.Type == QuestionType.Auction)
            {
                AuctionSystem.StartNew(PlayersBoard.Current, MatchData.SelectedRoundQuestion.Price);
            }
            else if (netQuestion.Type == QuestionType.CatInBag)
            {
                //todo: Finish refactoring
                //CatInBagData.IsPlayerSelected.Value = false;
                //SendToPlayersService.SendCatInBagData(CatInBagData);
            }

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
            MasterQuestionPanelView.RefreshUI();
        }
        
        public bool CanShowNext()
        {
            //todo: finish refactoring
            return true;
            //bool isWaitingWhoGetCatInBag = Data.CurrentStoryDot is CatInBagStoryDot && !CatInBagData.IsPlayerSelected.Value;
            //return !Data.IsLastDot && !isWaitingWhoGetCatInBag;
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

        public bool CanShowPrevious()
        {
            //todo: finish refactoring
            return true;
            
            //return Data.CurrentStoryDotIndex > 0 &&
            //       !(Data.PreviousStoryDot is CatInBagStoryDot) &&
            //       !(Data.PreviousStoryDot is NoRiskStoryDot);
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
            Data.PlayersButtonClickData.Clear();
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
            Data.TimerState = QuestionTimerState.Paused;
            //todo: finish refactoring
            //Data.Phase.Value = QuestionPhase.ShowAnswer;
            //todo: finish refactoring
            //Data.CurrentStoryDotIndex = 0;
            SendData(MasterIntention.ShowAnswer);
            Data.PlayersButtonClickData.Clear();
        }

        public void OnPlayerButtonClickReceived(byte playerId, float spentSeconds)
        {
            if (Data.TimerState == QuestionTimerState.NotStarted)
                return;
            
            bool wasReceivedBefore = Data.PlayersButtonClickData.Players.Any(_ => _.PlayerId == playerId);
            if (wasReceivedBefore)
                return;

            bool isNotCurrentForNoRiskQuestion = Data.QuestionType == QuestionType.NoRisk &&
                                                 PlayersBoard.Current != null &&
                                                 PlayersBoard.Current.PlayerId != playerId;
            if (isNotCurrentForNoRiskQuestion)
                return;

            bool didWrongAnswerBefore = Data.WrongAnsweredIds.Contains(playerId);
            if (didWrongAnswerBefore)
                return;
            
            Data.PlayersButtonClickData.Add(playerId, PlayersBoardSystem.GetPlayer(playerId).Name, spentSeconds);
            
            PauseTimer();

            MasterQuestionPanelView.RefreshUI();
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
            PlayerButtonClickData fastest = Data.PlayersButtonClickData.Players.OrderBy(_ => _.Time).FirstOrDefault();
            
            if (fastest == null)
                throw new Exception("Can't select fastest when list of players is empty.");

            SelectPlayerForAnswer(fastest.PlayerId);
        }
        
        public void SelectPlayerForAnswer(byte playerId)
        {
            if (NetworkData.IsClient)
                return;
            
            Data.AnsweringPlayerId = playerId;
            //todo: finish refactoring
            //Data.Phase.Value = QuestionPhase.AcceptingAnswer;
            SendToPlayersService.Send(Data);
            Data.PlayersButtonClickData.Clear();
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
            //todo: finish refactoring
            //Data.Phase.Value = QuestionPhase.ShowQuestion;
            StartTimer();
            SendData(MasterIntention.ContinueTimer);
        }

        public void AcceptAnswerAsCorrect()
        {
            ShowAnswer();
            PlayersBoardSystem.MakePlayerCurrent(Data.AnsweringPlayerId);
            MatchSystem.RewardPlayer(Data.AnsweringPlayerId);
        }

        public void AcceptAnswerAsWrong()
        {
            if (Data.SelectedQuestion.Value.Type == QuestionType.Simple)
            {
                Data.WrongAnsweredIds.Add(Data.AnsweringPlayerId);
                //todo: finish refactoring
                //Data.Phase.Value = QuestionPhase.ShowQuestion;
                StartTimer();
                SendData(MasterIntention.ContinueTimer);
                MatchSystem.FinePlayer(Data.AnsweringPlayerId);
            }
            else if (Data.SelectedQuestion.Value.Type == QuestionType.NoRisk)
            {
                ShowAnswer();
            }
            else if (Data.SelectedQuestion.Value.Type == QuestionType.CatInBag)
            {
                MatchSystem.FinePlayer(Data.AnsweringPlayerId);
                ShowAnswer();
            }
            else if (Data.SelectedQuestion.Value.Type == QuestionType.Auction)
            {
                MatchSystem.FinePlayer(Data.AnsweringPlayerId);
                ShowAnswer();
            }
        }
        
        #endregion
    }
}