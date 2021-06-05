using System.Collections.Generic;

namespace Victorina
{
    public class Round
    {
        public string Name { get; set; }
        public RoundType Type { get; set; }
        public List<Theme> Themes { get; set; } = new List<Theme>();
    }
}