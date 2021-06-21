using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class MakeBetForPlayerAuctionCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        
        public int Bet { get; set; }
        public PlayerData Player { get; set; }
        
        public override CommandType Type => CommandType.MakeBetForPlayerAuction;
        private AuctionPlayState AuctionPlayState => PlayStateData.As<AuctionPlayState>();
        
        public bool CanExecuteOnServer()
        {
            return true;
        }

        public void ExecuteOnServer()
        {
            AuctionPlayState.Bet = Bet;
            AuctionPlayState.IsAllIn = false;
            AuctionPlayState.Player = Player;
            AuctionPlayState.PassedPlayers.Remove(Player);
            AuctionPlayState.BettingPlayer = Player;
            AuctionSystem.AddAutomaticPasses();
            AuctionPlayState.BettingPlayer = AuctionSystem.SelectNextBettingPlayer();
            
            PlayStateData.MarkAsChanged();
        }
        
        public override string ToString()
        {
            return $"[MakeBetForPlayerAuctionCommand, Bet: {Bet}, Player: {Player}]";
        }
    }
}