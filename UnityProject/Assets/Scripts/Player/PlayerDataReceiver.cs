using Injection;

namespace Victorina
{
    public class PlayerDataReceiver
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private PlayerAnswerSystem PlayerAnswerSystem { get; set; }
        
        public void OnReceive(PlayersButtonClickData data)
        {
            MatchData.QuestionAnswerData.PlayersButtonClickData.Value = data;
        }

        public void OnReceive(QuestionPhase questionPhase)
        {
            QuestionAnswerData.Phase.Value = questionPhase;
        }
        
        public void OnReceiveStartTimer(float resetSeconds, float leftSeconds)
        {
            MatchData.Player.TimerResetSeconds = resetSeconds;
            MatchData.Player.TimerLeftSeconds = leftSeconds;
            PlayerAnswerSystem.EnableAnswer(resetSeconds, leftSeconds);
        }

        public void OnReceiveStopTimer()
        {
            PlayerAnswerSystem.StopTimer();
        }
    }
}