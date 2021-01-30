using Injection;

namespace Victorina
{
    public class PlayerDataReceiver
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PlayerAnswerSystem PlayerAnswerSystem { get; set; }
        
        public void OnReceive(PlayersButtonClickData data)
        {
            MatchData.PlayersButtonClickData.Value = data;
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