using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class FinishAuctionCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }

        public override CommandType Type => CommandType.FinishAuction;
        private AuctionPlayState AuctionPlayState => PlayStateData.As<AuctionPlayState>();

        public bool CanExecuteOnServer()
        {
            if (AuctionPlayState.Player == null)
            {
                Debug.Log("Auction player is null. Can't finish auction.");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            QuestionAnswerData.AdmittedPlayersIds.Add(AuctionPlayState.Player.PlayerId);
            PlayersBoardSystem.MakePlayerCurrent(AuctionPlayState.Player);
            QuestionAnswerSystem.StartQuestionStory();

            ShowQuestionPlayState showQuestionPlayState = new ShowQuestionPlayState();
            PlayStateSystem.ChangePlayState(showQuestionPlayState);
        }

        public override string ToString()
        {
            return "[FinishAuctionCommand]";
        }
    }
}