using Injection;

namespace Victorina
{
    public class MasterDataReceiver
    {
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }

        public void OnPlayerButtonClickReceived(ulong playerId, float thoughtSeconds)
        {
            QuestionAnswerSystem.OnPlayerButtonClickReceived(playerId, thoughtSeconds);
        }
    }
}