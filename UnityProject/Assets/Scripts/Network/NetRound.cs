using System.Collections.Generic;
using System.Linq;

namespace Victorina
{
    public class NetRound
    {
        public List<NetRoundTheme> Themes { get; } = new List<NetRoundTheme>();

        public override string ToString()
        {
            return $"NetRound: ({string.Join(",", Themes.Select(_ => _.Name))})";
        }
    }
}