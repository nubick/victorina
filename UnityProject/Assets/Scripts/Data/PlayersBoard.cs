using System.Collections.Generic;

namespace Victorina
{
    public class PlayersBoard
    {
        public readonly List<string> PlayerNames = new List<string>();

        public override string ToString()
        {
            return string.Join(",", PlayerNames);
        }
    }
}