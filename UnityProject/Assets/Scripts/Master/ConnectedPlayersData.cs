using System.Collections.Generic;

namespace Victorina
{
    public class ConnectedPlayersData
    {
        public Dictionary<ulong, string> PlayersIdNameMap { get; } = new Dictionary<ulong, string>();
    }
}