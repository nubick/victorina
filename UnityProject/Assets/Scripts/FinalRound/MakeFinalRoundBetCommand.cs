using System;
using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class MakeFinalRoundBetCommand : Command, INetworkCommand, IServerCommand
    {
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        public int Bet { get; set; }

        public override CommandType Type => CommandType.MakeFinalRoundBet;
        private FinalRoundPlayState PlayState => PlayStateData.As<FinalRoundPlayState>();
        
        public bool CanSend()
        {
            return CanExecuteOnServer();
        }

        public bool CanExecuteOnServer()
        {
            PlayerData bettingPlayer = GetBettingPlayer();
            if (Bet <= 0 || Bet > bettingPlayer.Score)
            {
                Debug.Log($"Can't accept bet '{Bet}' from player '{bettingPlayer}'.");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            PlayerData bettingPlayer = GetBettingPlayer();
            Debug.Log($"Make bet '{Bet}' for player '{bettingPlayer}'");
            int index = PlayersBoard.GetPlayerIndex(bettingPlayer);
            PlayState.SetBet(index, Bet);
        }

        private PlayerData GetBettingPlayer()
        {
            switch (Owner)
            {
                case CommandOwner.Master:
                    return PlayState.SelectedPlayerByMaster;
                case CommandOwner.Player:
                    return OwnerPlayer;
                default:
                    throw new Exception($"Not supported Owner: {Owner}");
            }
        }
        
        #region Serialization

        public void Serialize(PooledBitWriter writer)
        {
            writer.WriteInt32(Bet);
        }

        public void Deserialize(PooledBitReader reader)
        {
            Bet = reader.ReadInt32();
        }

        #endregion
        
        public override string ToString()
        {
            return $"[MakeFinalRoundBetCommand, Bet: {Bet}, Owner: {OwnerString}]";
        }
    }
}