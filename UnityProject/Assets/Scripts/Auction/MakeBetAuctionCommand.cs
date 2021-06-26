using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class MakeBetAuctionCommand : Command, IServerCommand, INetworkCommand
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        
        public int Bet { get; set; }

        public override CommandType Type => CommandType.MakeBetAuction;
        private AuctionPlayState AuctionPlayState => PlayStateData.As<AuctionPlayState>();

        private bool CanBet()
        {
            bool isForceBet = AuctionPlayState.Player == null && OwnerPlayer.Score < Bet;
            bool canBet = AuctionPlayState.BettingPlayer == OwnerPlayer && !AuctionPlayState.IsAllIn && Bet >= AuctionPlayState.NextMinBet &&
                          (OwnerPlayer.Score >= Bet || isForceBet);
            if (!canBet)
            {
                Debug.Log($"Player '{OwnerPlayer}' can't do bet '{Bet}', {AuctionPlayState}");
                return false;
            }
            return true;
        }
        
        public bool CanSend()
        {
            return CanBet();
        }

        public bool CanExecuteOnServer()
        {
            return CanBet();
        }

        public void ExecuteOnServer()
        {
            AuctionPlayState.Bet = Bet;
            AuctionPlayState.Player = OwnerPlayer;
            AuctionSystem.AddAutomaticPasses();
            AuctionPlayState.BettingPlayer = AuctionSystem.SelectNextBettingPlayer();
            
            PlayStateData.MarkAsChanged();
        }

        public void Serialize(PooledBitWriter writer)
        {
            writer.WriteInt32(Bet);
        }

        public void Deserialize(PooledBitReader reader)
        {
            Bet = reader.ReadInt32();
        }
        
        public override string ToString()
        {
            return $"[MakeBetAuctionCommand, Bet: {Bet}]";
        }
    }
}