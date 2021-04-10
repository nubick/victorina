using System.Collections.Generic;

namespace Victorina
{
    public class AuctionData
    {
        public int Bet { get; set; }
        public bool IsAllIn { get; set; }
        public PlayerData Player { get; set; }
        public PlayerData BettingPlayer { get; set; }
        public List<PlayerData> PassedPlayers { get; } = new List<PlayerData>();

        public bool IsFinished => BettingPlayer == Player;
        public int NextMinBet => Player == null ? Bet : Bet + Static.AuctionMinStep;

        public override string ToString()
        {
            return $"[Bet:{Bet}|AllIn:{IsAllIn}|Player:{(Player == null ? "None" : Player.Name)}|Betting:{BettingPlayer}|Passed:{string.Join(",", PassedPlayers)}]";
        }
    }
}