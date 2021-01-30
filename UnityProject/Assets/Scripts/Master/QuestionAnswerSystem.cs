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

        public void StartAnswer(NetQuestion netQuestion)
        {
            QuestionTimer.Reset(Static.TimeForAnswer);
            Data.WasTimerStarted = false;
            Data.IsTimerOn = false;

            Data.SelectedQuestion.Value = netQuestion;
            SendToPlayersService.SendSelectedQuestion(Data.SelectedQuestion.Value);

            Data.Phase.Value = QuestionPhase.ShowQuestion;
            SendToPlayersService.Send(Data.Phase.Value);
            
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
            SendToPlayersService.Send(Data.Phase.Value);

            Data.CurrentStoryDotIndex.Value = 0;
            SendToPlayersService.SendCurrentStoryDotIndex(Data.CurrentStoryDotIndex.Value);
            
            MasterQuestionPanelView.RefreshUI();
        }

        public void OnPlayerButtonClickReceived(ulong playerId, float thoughtSeconds)
        {
            PlayerButtonClickData clickData = Data.PlayersButtonClickData.Value.Players.SingleOrDefault(_ => _.PlayerId == playerId);
            if (clickData == null)
            {
                clickData = new PlayerButtonClickData();
                Data.PlayersButtonClickData.Value.Players.Add(clickData);
            }

            clickData.PlayerId = playerId;
            clickData.Name = ConnectedPlayersData.PlayersIdNameMap[playerId];
            clickData.Time = thoughtSeconds;
            
            Data.PlayersButtonClickData.NotifyChanged();
            SendToPlayersService.SendPlayersButtonClickData(Data.PlayersButtonClickData.Value);
            
            StopTimer();
            
            MasterQuestionPanelView.RefreshUI();
        }

        public void BackToRound()
        {
            MatchSystem.BackToRound();
        }
    }
}