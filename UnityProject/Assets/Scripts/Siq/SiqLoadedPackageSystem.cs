using System.IO;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class SiqLoadedPackageSystem
    {
        [Inject] private SiqLoadedPackageData Data { get; set; }
        [Inject] private PathData PathData { get; set; }
        
        public void Refresh()
        {
            string[] fullPaths = Directory.GetDirectories(PathData.PackagesPath);

            Data.PackagesPaths.Clear();
            Data.PackagesPaths.AddRange(fullPaths);
         
            Data.PackagesNames.Clear();
            Data.PackagesNames.AddRange(fullPaths.Select(Path.GetFileName));
            
            Debug.Log($"Detected (converted before) packs amount: {Data.PackagesNames.Count}");
        }
    }
}