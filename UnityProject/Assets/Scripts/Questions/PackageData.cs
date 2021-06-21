using System.Collections.Generic;

namespace Victorina
{
    public class PackageData
    {
        public Package Package { get; set; }
        public PackageProgress PackageProgress { get; set; }
        public List<NetRound> NetRounds { get; set; }
        public Dictionary<Round, NetRound> RoundsMap { get; set; }
    }
}