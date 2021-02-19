using System.Collections.Generic;
using System.Linq;

namespace Victorina
{
    public class PlayersButtonClickData
    {
        public List<PlayerButtonClickData> Players { get; } = new List<PlayerButtonClickData>();

        public override string ToString()
        {
            return string.Join("", Players.Select(_ => $"{_.Name}: {_.Time}"));
        }
    }
}