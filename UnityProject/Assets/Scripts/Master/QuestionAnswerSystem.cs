using System;
using System.Linq;
using Injection;

namespace Victorina
{
    public class QuestionAnswerSystem
    {
        [Inject] private QuestionAnswerData Data { get; set; }
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MasterQuestionPanelView MasterQuestionPanelView { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private DataChangeHandler DataChangeHandler { get; set; }
        
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
            
            if (IsLastQuestionStoryDot())
                StartTimer();
        
            SendData(MasterIntention.StartAnswering);
        }
        
        private void SendData(MasterIntention intention)
        {
            Data.MasterIntention = intention;
            SendToPlayersService.Send(Data);
            DataChangeHandler.HandleMasterIntention(Data);
            MasterQuestionPanelView.RefreshUI();
        }
        
        public void ShowNext()
        {
            Data.CurrentStoryDotIndex++;
            
            if(IsLastQuestionStoryDot())
                StartTimer();
            
            SendData(MasterIntention.ShowStoryDot);
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
            
            Data.PlayersButtonClickData.Value.Players.Clear();
            Data.PlayersButtonClickData.NotifyChanged();
            SendToPlayersService.SendPlayersButtonClickData(Data.PlayersButtonClickData.Value);
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
        
        public void ShowAnswer()
        {
            Data.TimerState = QuestionTimerState.Paused;
            Data.Phase.Value = QuestionPhase.ShowAnswer;
            Data.CurrentStoryDotIndex = 0;
            
            SendData(MasterIntention.ShowAnswer);
        }

        public void OnPlayerButtonClickReceived(ulong playerId, float spentSeconds)
        {
            bool wasReceivedBefore = Data.PlayersButtonClickData.Value.Players.Any(_ => _.PlayerId == playerId);
            if (wasReceivedBefore)
                return;

            bool wrongAnsweredBefore = Data.WrongAnsweredIds.Contains(playerId);
            if (wrongAnsweredBefore)
                return;
            
            PlayerButtonClickData clickData = new PlayerButtonClickData();
            clickData.PlayerId = playerId;
            clickData.Name = ConnectedPlayersData.PlayersIdToNameMap[playerId];
            clickData.Time = spentSeconds;
            Data.PlayersButtonClickData.Value.Players.Add(clickData);

            Data.PlayersButtonClickData.NotifyChanged();
            SendToPlayersService.SendPlayersButtonClickData(Data.PlayersButtonClickData.Value);

            PauseTimer();

            MasterQuestionPanelView.RefreshUI();
        }

        public void BackToRound()
        {
            MatchSystem.BackToRound();
        }
        
        #region Accepting answer
        
        public void SelectPlayerForAnswer(ulong playerId)
        {
            if (NetworkData.IsClient)
                return;
            
            Data.AnsweringPlayerName = ConnectedPlayersData.PlayersIdToNameMap[playerId];
            Data.AnsweringPlayerId = playerId;
            Data.AnswerTip = GetAnswerTip(Data.SelectedQuestion.Value);
            Data.Phase.Value = QuestionPhase.AcceptingAnswer;
            SendToPlayersService.Send(Data);
        }
        
        private string GetAnswerTip(NetQuestion netQuestion)
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
            MatchSystem.RewardPlayer(Data.AnsweringPlayerId);
        }

        public void AcceptAnswerAsWrong()
        {
            Data.WrongAnsweredIds.Add(Data.AnsweringPlayerId);
            Data.Phase.Value = QuestionPhase.ShowQuestion;
            StartTimer();
            SendData(MasterIntention.ContinueTimer);
            MatchSystem.FinePlayer(Data.AnsweringPlayerId);
        }
        
        #endregion
    }
}