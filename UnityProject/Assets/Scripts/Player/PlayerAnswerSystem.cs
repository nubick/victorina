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
            float leftSeconds = QuestionTimer.LeftSeconds;
            float thoughtSeconds = MatchData.Player.TimerLeftSeconds - leftSeconds;
            SendToMasterService.SendPlayerButton(thoughtSeconds);
        }
    }
}