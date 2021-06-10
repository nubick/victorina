using System;
using System.Collections.Generic;

namespace Victorina
{
    public class PlayersBoard : SyncData
    {
        public List<PlayerData> Players { get; } = new List<PlayerData>();
        public PlayerData Current { get; private set; }

        public void SetCurrent(PlayerData player)
        {
            Current = player;
            MarkAsChanged();
        }

        public void Clear()
        {
            Players.Clear();
            Current = null;
            MarkAsChanged();
        }

        public void Update(PlayersBoard playersBoard)
        {
            Players.Rewrite(playersBoard.Players);
            Current = playersBoard.Current;
            MarkAsChanged();
        }

        public int GetPlayerIndex(PlayerData player)
        {
            if (Players.Contains(player))
                return Players.IndexOf(player);

            throw new Exception($"PlayersBoard doesn't contain player: {player}");
        }

        public override string ToString()
        {
            return $"[PB, amount: {Players.Count}, {string.Join(",", Players)}, cur: {(Current == null ? "none" : Current.ToString())}]";
        }
    }
}