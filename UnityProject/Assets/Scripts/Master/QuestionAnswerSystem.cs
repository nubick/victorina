using System;
using System.Linq;
using Injection;
using UnityEngine;

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
        [Inject] private CatInBagData CatInBagData { get; set; }
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        
        private bool IsLastQuestionStoryDot() => Data.TimerState == QuestionTimerState.NotStarted &&
                                                 Data.Phase.Value == QuestionPhase.ShowQuestion &&
                                                 Data.CurrentStoryDot == Data.SelectedQuestion.Value.QuestionStory.Last();

        public void StartAnswer(NetQuestion netQuestion)
        {
            Data.SelectedQuestion.Value = netQuestion;
            SendToPlayersService.SendSelectedQuestion(Data.SelectedQuestion.Value);

            QuestionTimer.Reset(Static.TimeForAnswer);

            Data.TimerState = QuestionTimerState.NotStarted;
            Data.WrongAnsweredIds.Clear();
            Data.Phase.Value = QuestionPhase.ShowQuestion;
            Data.CurrentStoryDotIndex = 0;
            
            Data.AnswerTip = GetAnswerTip(Data.SelectedQuestion.Value);
            Data.IsAnswerTipEnabled = false;
            
            if (IsLastQuestionStoryDot())
                StartTimer();

            if (Data.CurrentStoryDot is CatInBagStoryDot)
            {
                CatInBagData.IsPlayerSelected.Value = false;
                SendToPlayersService.SendCatInBagData(CatInBagData);
            }

            if (Data.CurrentStoryDot is AuctionStoryDot)
            {
                AuctionSystem.StartNew(MatchData.PlayersBoard.Value.Current, MatchData.SelectedRoundQuestion.Price);
            }
            
            
            SendData(MasterIntention.StartAnswering);
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
            bool isWaitingWhoGetCatInBag = Data.CurrentStoryDot is CatInBagStoryDot && !CatInBagData.IsPlayerSelected.Value;
            return !Data.IsLastDot && !isWaitingWhoGetCatInBag;
        }
        
        public void ShowNext()
        {
            Data.CurrentStoryDotIndex++;
            
            if(IsLastQuestionStoryDot())
                StartTimer();
            
            SendData(MasterIntention.ShowStoryDot);
        }

        public bool CanShowPrevious()
        {
            return Data.CurrentStoryDotIndex > 0 && !(Data.PreviousStoryDot is CatInBagStoryDot) && !(Data.PreviousStoryDot is NoRiskStoryDot);
        }
        
        public void ShowPrevious()
        {
            Data.CurrentStoryDotIndex--;
            SendData(MasterIntention.ShowStoryDot);
        }
        
        private void StartTimer()
        {
            Data.TimerResetSeconds = Static.TimeForAnswer;
            Data.TimerLeftSeconds = QuestionTimer.LeftSeconds;
            Data.TimerState = QuestionTimerState.Running;
            ClearPlayersButtonClickData();
        }

        private void ClearPlayersButtonClickData()
        {
            if (Data.PlayersButtonClickData.Value.Players.Any())
            {
                Data.PlayersButtonClickData.Value.Players.Clear();
                Data.PlayersButtonClickData.NotifyChanged();
                SendToPlayersService.SendPlayersButtonClickData(Data.PlayersButtonClickData.Value);
            }
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
            return Data.SelectedQuestion.Value.Type == QuestionType.Simple && Data.Phase.Value == QuestionPhase.ShowQuestion && Data.TimerState != QuestionTimerState.NotStarted;
        }
        
        public void ShowAnswer()
        {
            Data.TimerState = QuestionTimerState.Paused;
            Data.Phase.Value = QuestionPhase.ShowAnswer;
            Data.CurrentStoryDotIndex = 0;
            
            SendData(MasterIntention.ShowAnswer);
            
            ClearPlayersButtonClickData();
        }

        public void OnPlayerButtonClickReceived(byte playerId, float spentSeconds)
        {
            if (Data.TimerState == QuestionTimerState.NotStarted)
                return;
            
            bool wasReceivedBefore = Data.PlayersButtonClickData.Value.Players.Any(_ => _.PlayerId == playerId);
            if (wasReceivedBefore)
                return;

            bool isNotCurrentForNoRiskQuestion = Data.QuestionType == QuestionType.NoRisk &&
                                                 MatchData.PlayersBoard.Value.Current != null &&
                                                 MatchData.PlayersBoard.Value.Current.PlayerId != playerId;
            if (isNotCurrentForNoRiskQuestion)
                return;

            bool didWrongAnswerBefore = Data.WrongAnsweredIds.Contains(playerId);
            if (didWrongAnswerBefore)
                return;
            
            PlayerButtonClickData clickData = new PlayerButtonClickData();
            clickData.PlayerId = playerId;
            clickData.Name = PlayersBoardSystem.GetPlayer(playerId).Name;
            clickData.Time = spentSeconds;
            Data.PlayersButtonClickData.Value.Players.Add(clickData);

            Data.PlayersButtonClickData.NotifyChanged();
            SendToPlayersService.SendPlayersButtonClickData(Data.PlayersButtonClickData.Value);

            PauseTimer();

            MasterQuestionPanelView.RefreshUI();
        }

        public bool CanBackToRound()
        {
            return Data.Phase.Value == QuestionPhase.ShowAnswer && Data.IsLastDot;
        }
        
        public void BackToRound()
        {
            MatchSystem.BackToRound();
        }
        
        #region Accepting answer

        public void SelectFastestPlayerForAnswer()
        {
            PlayerButtonClickData fastest = Data.PlayersButtonClickData.Value.Players.OrderBy(_ => _.Time).FirstOrDefault();
            
            if (fastest == null)
                throw new Exception("Can't select fastest when list of players is empty.");

            SelectPlayerForAnswer(fastest.PlayerId);
        }
        
        public void SelectPlayerForAnswer(byte playerId)
        {
            if (NetworkData.IsClient)
                return;
            
            Data.AnsweringPlayerName = PlayersBoardSystem.GetPlayer(playerId).Name;
            Data.AnsweringPlayerId = playerId;
            Data.Phase.Value = QuestionPhase.AcceptingAnswer;
            SendToPlayersService.Send(Data);
            
            ClearPlayersButtonClickData();
        }

        public void AcceptNoRiskAnswer()
        {
            if (MatchData.PlayersBoard.Value.Current == null)
            {
                Debug.Log("Master. Error. Can't accept no risk answer. Current player is null.");
            }
            else
            {
                PauseTimer();
                SelectPlayerForAnswer(MatchData.PlayersBoard.Value.Current.PlayerId);
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
            Data.Phase.Value = QuestionPhase.ShowQuestion;
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
                Data.Phase.Value = QuestionPhase.ShowQuestion;
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
        }
        
        #endregion
    }
}