using System.Collections.Generic;

namespace Victorina
{
    public class ConnectedPlayersData
    {
        public Dictionary<ulong, ConnectionMessage> PlayersIdToConnectionMessageMap { get; } = new Dictionary<ulong, ConnectionMessage>();
        public List<ulong> ConnectedClientsIds { get; } = new List<ulong>();
    }
}