using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class FinishAuctionCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }

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
            PlayersBoardSystem.MakePlayerCurrent(AuctionPlayState.Player);
            PlayStateSystem.ChangeToShowQuestionPlayState(MatchData.NetQuestion, AuctionPlayState.Bet);
        }

        public override string ToString()
        {
            return "[FinishAuctionCommand]";
        }
    }
}