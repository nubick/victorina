using System;
using System.Linq;
using Injection;

namespace Victorina
{
    public class PlayerAnswerSystem
    {
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private SendToMasterService SendToMasterService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        
        public bool IsTimerRunning => QuestionTimer.IsRunning;
        
        public void StartTimer(float resetSeconds, float leftSeconds)
        {
            MatchData.Player.EnableAnswerTime = DateTime.UtcNow;
            QuestionTimer.Reset(resetSeconds, leftSeconds);
            QuestionTimer.Start();
        }
        
        public void StopTimer()
        {
            QuestionTimer.Stop();
        }

        private void SendAnswerIntention()
        {
            if (!CanSendAnswerIntention())
                return;
            
            float spentSeconds = (float) (DateTime.UtcNow - MatchData.Player.EnableAnswerTime).TotalSeconds;
            SendToMasterService.SendPlayerButton(spentSeconds);
        }

        public void OnAnswerButtonClicked()
        {
            SendAnswerIntention();
        }
        
        public void OnAnyKeyDown()
        {
            SendAnswerIntention();
        }

        public bool CanSendAnswerIntention()
        {
            return NetworkData.IsClient && MatchData.Phase.Value == MatchPhase.Question &&
                   QuestionAnswerData.Phase.Value == QuestionPhase.ShowQuestion &&
                   !WasIntentionSent() && !WasWrongAnswer();
        }

        public bool WasWrongAnswer()
        {
            return QuestionAnswerData.WrongAnsweredIds.Contains(NetworkData.PlayerId);
        }

        public bool WasIntentionSent()
        {
            return QuestionAnswerData.PlayersButtonClickData.Value.Players.Any(_ => _.PlayerId == NetworkData.PlayerId);
        }
    }
}