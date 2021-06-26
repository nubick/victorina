using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class PassAuctionCommand : Command, IServerCommand, INetworkCommand
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        
        public override CommandType Type => CommandType.PassAuction;
        private AuctionPlayState PlayState => PlayStateData.As<AuctionPlayState>();
        
        public bool CanSend()
        {
            return CanExecuteOnServer();
        }
        
        public bool CanExecuteOnServer()
        {
            if (!AuctionSystem.CanPass(OwnerPlayer))
            {
                Debug.Log($"Player '{OwnerPlayer}' can't pass as it is betting player and player was not selected yet.");
                return false;
            }
            
            if (PlayState.PassedPlayers.Contains(OwnerPlayer))
            {
                Debug.Log($"Receive pass request from player '{OwnerPlayer}' who passed before");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            PlayState.PassedPlayers.Add(OwnerPlayer);
            if (PlayState.BettingPlayer == OwnerPlayer)
                PlayState.BettingPlayer = AuctionSystem.SelectNextBettingPlayer();
            
            PlayStateData.MarkAsChanged();
        }

        public void Serialize(PooledBitWriter writer) { }
        public void Deserialize(PooledBitReader reader) { }
        
        public override string ToString()
        {
            return "[PassAuctionCommand]";
        }
    }
}