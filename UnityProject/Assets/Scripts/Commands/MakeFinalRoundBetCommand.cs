using System;
using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;

namespace Victorina.Commands
{
    public class MakeFinalRoundBetCommand : Command, INetworkCommand, IServerCommand
    {
        [Inject] private FinalRoundData FinalRoundData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        
        public int Bet { get; set; }

        public override CommandType Type => CommandType.MakeFinalRoundBet;
        
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
            FinalRoundData.SetBet(index, Bet);
        }

        private PlayerData GetBettingPlayer()
        {
            switch (Owner)
            {
                case CommandOwner.Master:
                    return FinalRoundData.SelectedPlayerByMaster;
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