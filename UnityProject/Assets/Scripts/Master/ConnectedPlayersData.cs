using System;
using System.Collections.Generic;
using System.Linq;

namespace Victorina
{
    public class ConnectedPlayersData
    {
        public byte NextPlayerId { get; set; }
        public List<JoinedPlayer> Players { get; } = new List<JoinedPlayer>();
        public Dictionary<ulong, PlayerRejectReason> RejectedPlayers { get; } = new Dictionary<ulong, PlayerRejectReason>();
        public Queue<ulong> WaitingFirstMessageClientIds { get; } = new Queue<ulong>();

        public void Clear()
        {
            NextPlayerId = 1;
            Players.Clear();
            RejectedPlayers.Clear();
            WaitingFirstMessageClientIds.Clear();
        }
        
        public byte GetPlayerId(ulong clientId)
        {
            JoinedPlayer player = GetByClientId(clientId);
            if (player == null)
                throw new Exception($"Can't find JoinedPlayer by client id: {clientId}");
            return player.PlayerId;
        }

        public ulong GetClientId(byte playerId)
        {
            JoinedPlayer player = GetByPlayerId(playerId);
            if (player == null)
                throw new Exception($"Can't find JoinedPlayer by player id: {playerId}");
            return player.ClientId;
        }

        public JoinedPlayer GetByClientId(ulong clientId)
        {
            return Players.SingleOrDefault(_ => _.ClientId == clientId);
        }

        public JoinedPlayer GetByPlayerId(byte playerId)
        {
            return Players.SingleOrDefault(_ => _.PlayerId == playerId);
        }

        public JoinedPlayer GetByGuid(string guid)
        {
            return Players.SingleOrDefault(_ => _.Guid == guid);
        }
    }
}