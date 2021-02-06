using System;
using Injection;

namespace Victorina
{
    public class PlayerAnswerSystem
    {
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private SendToMasterService SendToMasterService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
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
        
        public void SendAnswer()
        {
            QuestionTimer.Stop();

            float spentSeconds = (float) (DateTime.UtcNow - MatchData.Player.EnableAnswerTime).TotalSeconds;
            SendToMasterService.SendPlayerButton(spentSeconds);
        }
    }
}