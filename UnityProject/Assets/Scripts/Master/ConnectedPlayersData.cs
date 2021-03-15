using System;
using System.Collections.Generic;
using System.Linq;

namespace Victorina
{
    public class ConnectedPlayersData
    {
        public byte NextPlayerId { get; set; }
        public List<JoinedPlayer> Players { get; } = new List<JoinedPlayer>();

        public void Clear()
        {
            NextPlayerId = 1;
            Players.Clear();
        }
        
        public byte GetPlayerId(ulong clientId)
        {
            JoinedPlayer player = Players.SingleOrDefault(_ => _.ClientId == clientId);
            if (player == null)
                throw new Exception($"Can't find JoinedPlayer by client id: {clientId}");
            return player.PlayerId;
        }
    }
}