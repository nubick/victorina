using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class MakeAllInAuctionCommand : Command, IServerCommand, INetworkCommand
    {
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        
        public override CommandType Type => CommandType.MakeAllInAuction;
        private AuctionPlayState AuctionPlayState => PlayStateData.As<AuctionPlayState>();

        private bool CanAllIn()
        {
            bool canAllIn = AuctionPlayState.BettingPlayer == OwnerPlayer && OwnerPlayer.Score >= AuctionPlayState.NextMinBet;
            if (!canAllIn)
            {
                Debug.Log($"Player '{OwnerPlayer}' can't all in {AuctionPlayState}");
                return false;
            }

            return true;
        }
        
        public bool CanSend()
        {
            return CanAllIn();
        }
        
        public bool CanExecuteOnServer()
        {
            return CanAllIn();
        }

        public void ExecuteOnServer()
        {
            AuctionPlayState.IsAllIn = true;
            AuctionPlayState.Bet = OwnerPlayer.Score;
            AuctionPlayState.Player = OwnerPlayer;
            AuctionSystem.AddAutomaticPasses();
            AuctionPlayState.BettingPlayer = AuctionSystem.SelectNextBettingPlayer();
            
            PlayStateData.MarkAsChanged();
        }

        public void Serialize(PooledBitWriter writer) { }
        public void Deserialize(PooledBitReader reader) { }
        
        public override string ToString()
        {
            return "[MakeAllInAuctionCommand]";
        }
    }
}