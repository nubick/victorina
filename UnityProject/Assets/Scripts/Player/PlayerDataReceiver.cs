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

        public void OnReceive(QuestionAnswerData data)
        {
            QuestionAnswerData.AnsweringPlayerId = data.AnsweringPlayerId;
            QuestionAnswerData.AnsweringPlayerName = data.AnsweringPlayerName;
            QuestionAnswerData.WrongAnsweredIds = data.WrongAnsweredIds;

            if (QuestionAnswerData.Phase.Value != data.Phase.Value)
                QuestionAnswerData.Phase.Value = data.Phase.Value;
        }

        public void OnReceiveStartTimer(float resetSeconds, float leftSeconds)
        {
            PlayerAnswerSystem.EnableAnswer(resetSeconds, leftSeconds);
        }

        public void OnReceiveStopTimer()
        {
            PlayerAnswerSystem.StopTimer();
        }
    }
}