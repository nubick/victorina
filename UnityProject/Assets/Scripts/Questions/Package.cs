using System.Collections.Generic;

namespace Victorina
{
    public class Package
    {
        public string Name { get; set; }
        public List<Round> Rounds { get; set; } = new List<Round>();

        public Package(string name)
        {
            Name = name;
        }
    }
}