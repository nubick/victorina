using System.Collections.Generic;
using System.IO;
using System.Linq;
using Injection;

namespace Victorina
{
    public class PackageEditorSystem
    {
        [Inject] private PathData PathData { get; set; }
        
        public List<string> GetOpenedPackagesNames()
        {
            string[] fullPaths = Directory.GetDirectories(PathData.PackageEditorPath);
            return fullPaths.Select(Path.GetFileName).ToList();
        }
        
    }
}