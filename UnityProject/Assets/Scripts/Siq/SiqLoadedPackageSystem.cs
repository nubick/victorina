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
            Data.PackageNames.Clear();
            string[] fullPaths = Directory.GetDirectories(PathData.PackagesPath);
            Data.PackageNames.AddRange(fullPaths.Select(Path.GetFileName));
            Debug.Log($"Detected (converted before) packs amount: {Data.PackageNames.Count}");
        }

        public void Delete(string packageName)
        {
            string path = $"{PathData.PackagesPath}/{packageName}";
            Debug.Log($"Delete directory: {path}");
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Debug.Log("Deletion is done.");
            }
            else
            {
                Debug.Log($"Directory doesn't exist.");
            }
        }
    }
}