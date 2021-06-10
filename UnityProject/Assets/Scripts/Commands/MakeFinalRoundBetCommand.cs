using System;
using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;

namespace Victorina.Commands
{
    public class MakeFinalRoundBetCommand : CommandBase
    {
        [Inject] private FinalRoundData FinalRoundData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        
        public int Bet { get; set; }

        public override CommandType Type => CommandType.MakeFinalRoundBet;
        
        public override bool CanSendToServer()
        {
            return CanExecuteOnServer();
        }

        public override bool CanExecuteOnServer()
        {
            PlayerData bettingPlayer = GetBettingPlayer();
            if (Bet <= 0 || Bet > bettingPlayer.Score)
            {
                Debug.Log($"Can't accept bet '{Bet}' from player '{bettingPlayer}'.");
                return false;
            }

            return true;
        }

        public override void ExecuteOnServer()
        {
            PlayerData bettingPlayer = GetBettingPlayer();
            Debug.Log($"Make bet '{Bet}' for player '{bettingPlayer}'");
            int index = PlayersBoard.Players.IndexOf(bettingPlayer);
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

        public override void Serialize(PooledBitWriter writer)
        {
            writer.WriteInt32(Bet);
        }

        public override void Deserialize(PooledBitReader reader)
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