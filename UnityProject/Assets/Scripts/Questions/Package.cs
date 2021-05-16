using System.Collections.Generic;

namespace Victorina
{
    public class Package
    {
        public string Path { get; set; }
        public string FolderName => System.IO.Path.GetFileName(Path);
        public List<Round> Rounds { get; set; } = new List<Round>();
    }
}