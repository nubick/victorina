using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using Injection;
using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class AuctionPlayState : PackagePlayState
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        
        public int Bet { get; set; }
        public string Theme { get; set; }
        public bool IsAllIn { get; set; }
        public PlayerData Player { get; set; }
        public PlayerData BettingPlayer { get; set; }
        public List<PlayerData> PassedPlayers { get; } = new List<PlayerData>();

        //Master Only
        public PlayerData SelectedPlayerByMaster { get; set; }
        
        public bool IsFinished => BettingPlayer == Player;
        public int NextMinBet => Player == null ? Bet : Bet + Static.AuctionMinStep;
        public override PlayStateType Type => PlayStateType.Auction;
        
        public override void Serialize(PooledBitWriter writer)
        {
            writer.WriteInt32(Bet);
            writer.WriteString(Theme);
            writer.WriteBool(IsAllIn);
            WritePlayerData(writer, Player);
            WritePlayerData(writer, BettingPlayer);
            writer.WriteByteArray(PassedPlayers.Select(_ => _.PlayerId).ToArray());
        }

        public override void Deserialize(PooledBitReader reader)
        {
            Bet = reader.ReadInt32();
            Theme = reader.ReadString().ToString();
            IsAllIn = reader.ReadBool();
            Player = ReadPlayerData(reader);
            BettingPlayer = ReadPlayerData(reader);
            byte[] playerIds = reader.ReadByteArray();
            playerIds.ForEach(playerId => PassedPlayers.Add(PlayersBoardSystem.GetPlayer(playerId)));
        }
        
        private void WritePlayerData(PooledBitWriter writer, PlayerData playerData)
        {
            writer.WriteByte(playerData?.PlayerId ?? 0);
        }

        private PlayerData ReadPlayerData(PooledBitReader reader)
        {
            byte playerId = reader.ReadByteDirect();
            return playerId == 0 ? null : PlayersBoardSystem.GetPlayer(playerId);
        }
        
        public override string ToString()
        {
            return $"[AuctionPlayState, {nameof(Bet)}:{Bet}|{nameof(IsAllIn)}:{IsAllIn}|{nameof(Player)}:{(Player == null ? "None" : Player.Name)}|Betting:{BettingPlayer}|Passed:{string.Join(",", PassedPlayers)}]";
        }
    }
}