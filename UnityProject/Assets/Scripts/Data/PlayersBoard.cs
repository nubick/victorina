using System;
using System.Collections.Generic;
using System.Linq;

namespace Victorina
{
    public class PlayersBoard : SyncData
    {
        public List<PlayerData> Players { get; } = new List<PlayerData>();

        private PlayerData _current;
        public PlayerData Current
        {
            get => _current;
            private set
            {
                _current = value; 
                MarkAsChanged();
            }
        }

        public override bool HasChanges => base.HasChanges || Players.Any(_ => _.HasChanges);

        public override void ApplyChanges()
        {
            base.ApplyChanges();
            Players.ForEach(_ => _.ApplyChanges());
        }

        public void SetCurrent(PlayerData player)
        {
            Current = player;
        }

        public void Clear()
        {
            Players.Clear();
            Current = null;
        }

        public void Update(PlayersBoard playersBoard)
        {
            Players.Rewrite(playersBoard.Players);
            Current = playersBoard.Current;
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