using System;
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
        
        public void EnableAnswer(float resetSeconds, float leftSeconds)
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
            
            QuestionTimer.Stop();

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
                   QuestionAnswerData.Phase.Value == QuestionPhase.ShowQuestion && !WasWrongAnswer();
        }

        public bool WasWrongAnswer()
        {
            return QuestionAnswerData.WrongAnsweredIds.Contains(NetworkData.PlayerId);
        }
    }
}