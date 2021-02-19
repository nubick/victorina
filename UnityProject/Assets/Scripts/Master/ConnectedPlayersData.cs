using System.Collections.Generic;

namespace Victorina
{
    public class ConnectedPlayersData
    {
        public Dictionary<ulong, string> PlayersIdToNameMap { get; } = new Dictionary<ulong, string>();
        public List<ulong> ConnectedClientsIds { get; } = new List<ulong>();
    }
}