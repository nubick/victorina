using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class MakeBetForPlayerAuctionCommand : Command, IServerCommand
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        
        public int Bet { get; set; }
        public byte BettingPlayerId { get; set; }
        
        public override CommandType Type => CommandType.MakeBetForPlayerAuction;
        private AuctionPlayState AuctionPlayState => PlayStateData.As<AuctionPlayState>();
        
        public bool CanExecuteOnServer()
        {
            return true;
        }

        public void ExecuteOnServer()
        {
            PlayerData player = PlayersBoardSystem.GetPlayer(BettingPlayerId);
            
            AuctionPlayState.Bet = Bet;
            AuctionPlayState.IsAllIn = false;
            AuctionPlayState.Player = player;
            AuctionPlayState.PassedPlayers.Remove(player);
            AuctionPlayState.BettingPlayer = player;
            AuctionSystem.AddAutomaticPasses();
            AuctionPlayState.BettingPlayer = AuctionSystem.SelectNextBettingPlayer();
            
            PlayStateData.MarkAsChanged();
        }
        
        public override string ToString()
        {
            return $"[MakeBetForPlayerAuctionCommand, Bet: {Bet}, PlayerId: {BettingPlayerId}]";
        }
    }
}