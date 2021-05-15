using Injection;
using System.IO;
using UnityEngine;

namespace Victorina
{
    public class PackageFilesSystem
    {
        [Inject] private PathData PathData { get; set; }
        [Inject] private SiqConverter SiqConverter { get; set; }
        [Inject] private PackageJsonConverter PackageJsonConverter { get; set; }
        
        public string[] GetCrafterPackagesPaths()
        {
            return Directory.GetDirectories(PathData.CrafterPath);
        }
        
        public Package LoadPackage(string packagePath)
        {
            Package package;
            if (SiqConverter.IsValid(packagePath))
            {
                package = SiqConverter.Convert(packagePath);
            }
            else
            {
                string jsonPath = $"{packagePath}/package.json";
                string json = File.ReadAllText(jsonPath);
                package = PackageJsonConverter.ReadPackage(json);
            }
            package.Path = packagePath;
            return package;
        }

        public void Delete(string packagePath)
        {
            Debug.Log($"Delete package with path: {packagePath}");
            if (Directory.Exists(packagePath))
            {
                Directory.Delete(packagePath, true);
                Debug.Log("Package deletion is done.");
            }
            else
            {
                Debug.Log($"Package directory doesn't exist: {packagePath}");
            }
        }

        public void Delete(Package package)
        {
            Delete(package.Path);
        }
    }
}