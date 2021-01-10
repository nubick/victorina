using System.Collections.Generic;

namespace Victorina
{
    public class NetRoundTheme
    {
        public string Name { get; set; }
        public List<NetRoundQuestion> Questions { get; } = new List<NetRoundQuestion>();
    }
}