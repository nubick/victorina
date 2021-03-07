using System.Collections.Generic;

namespace Victorina
{
    public class PlayersBoard
    {
        public List<PlayerData> Players { get; } = new List<PlayerData>();
        public PlayerData Current { get; set; }

        public override string ToString()
        {
            return $"[PB, amount: {Players.Count}, cur: {(Current == null ? "none" : Current.ToString())}]";
        }
    }
}