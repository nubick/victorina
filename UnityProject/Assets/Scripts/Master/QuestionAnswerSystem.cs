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
        
        public void StartAnswer(NetQuestion netQuestion)
        {
            QuestionTimer.Reset(Static.TimeForAnswer);
            Data.WasTimerStarted = false;
            Data.IsTimerOn = false;
            Data.WrongAnsweredIds.Clear();

            Data.SelectedQuestion.Value = netQuestion;
            SendToPlayersService.SendSelectedQuestion(Data.SelectedQuestion.Value);

            Data.Phase.Value = QuestionPhase.ShowQuestion;
            SendToPlayersService.Send(Data);

            Data.CurrentStoryDotIndex.Value = 0;
            SendToPlayersService.SendCurrentStoryDotIndex(Data.CurrentStoryDotIndex.Value);
            
            StartTimerIfTime();
            
            MasterQuestionPanelView.RefreshUI();
        }
        
        private void StartTimerIfTime()
        {
            if (Data.WasTimerStarted)
                return;
            
            if (Data.Phase.Value == QuestionPhase.ShowQuestion &&
                Data.CurrentStoryDot == Data.SelectedQuestion.Value.QuestionStory.Last())
                StartTimer();
        }
        
        public void ShowNext()
        {
            Data.CurrentStoryDotIndex.Value++;
            SendToPlayersService.SendCurrentStoryDotIndex(Data.CurrentStoryDotIndex.Value);
            StartTimerIfTime();
            MasterQuestionPanelView.RefreshUI();
        }
        
        public void ShowPrevious()
        {
            Data.CurrentStoryDotIndex.Value--;
            SendToPlayersService.SendCurrentStoryDotIndex(Data.CurrentStoryDotIndex.Value);
            MasterQuestionPanelView.RefreshUI();
        }
        
        public void StartTimer()
        {
            Data.WasTimerStarted = true;
            Data.IsTimerOn = true;
            
            QuestionTimer.Start();
            SendToPlayersService.SendStartTimer(Static.TimeForAnswer, QuestionTimer.LeftSeconds);
            
            Data.PlayersButtonClickData.Value.Players.Clear();
            Data.PlayersButtonClickData.NotifyChanged();
            SendToPlayersService.SendPlayersButtonClickData(Data.PlayersButtonClickData.Value);
        }

        public void StopTimer()
        {
            QuestionTimer.Stop();
            
            Data.IsTimerOn = false;
            SendToPlayersService.SendStopTimer();
        }
        
        public void ShowAnswer()
        {
            StopTimer();
            Data.Phase.Value = QuestionPhase.ShowAnswer;
            SendToPlayersService.Send(Data);

            Data.CurrentStoryDotIndex.Value = 0;
            SendToPlayersService.SendCurrentStoryDotIndex(Data.CurrentStoryDotIndex.Value);
            
            MasterQuestionPanelView.RefreshUI();
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
            clickData.Name = ConnectedPlayersData.PlayersIdNameMap[playerId];
            clickData.Time = spentSeconds;
            Data.PlayersButtonClickData.Value.Players.Add(clickData);

            Data.PlayersButtonClickData.NotifyChanged();
            SendToPlayersService.SendPlayersButtonClickData(Data.PlayersButtonClickData.Value);

            StopTimer();

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
            
            Data.AnsweringPlayerName = ConnectedPlayersData.PlayersIdNameMap[playerId];
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
            SendToPlayersService.Send(Data);

            StartTimer();
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
            SendToPlayersService.Send(Data);
            StartTimer();
            MatchSystem.FinePlayer(Data.AnsweringPlayerId);
        }
        
        #endregion
    }
}