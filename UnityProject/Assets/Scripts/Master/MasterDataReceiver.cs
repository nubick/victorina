using Injection;

namespace Victorina
{
    public class MasterDataReceiver
    {
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }

        public void OnPlayerButtonClickReceived(ulong playerId, float spentSeconds)
        {
            QuestionAnswerSystem.OnPlayerButtonClickReceived(playerId, spentSeconds);
        }
    }
}