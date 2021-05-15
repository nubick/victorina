using System;
using Injection;
using System.IO;
using System.IO.Compression;
using SFB;
using UnityEngine;

namespace Victorina
{
    public class PackageFilesSystem
    {
        [Inject] private PathData PathData { get; set; }
        [Inject] private SiqConverter SiqConverter { get; set; }
        [Inject] private PackageJsonConverter PackageJsonConverter { get; set; }

        #region Archive file
        
        public string GetPackageArchivePathUsingOpenDialogue()
        {
            ExtensionFilter[] extensions =
            {
                new ExtensionFilter("Vumka Files, vum"),
                new ExtensionFilter("SIQ Files", "siq")
            };
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            return paths[0];
        }
        
        public string UnZipArchiveToPlayFolder(string packageArchivePath)
        {
            return UnZipArchiveToFolder(packageArchivePath, PathData.PackagesPath);
        }

        public string UnZipArchiveToCrafterFolder(string packageArchivePath)
        {
            return UnZipArchiveToFolder(packageArchivePath, PathData.CrafterPath);
        }

        private string UnZipArchiveToFolder(string packageArchivePath, string destinationFolderPath)
        {
            string archiveName = Path.GetFileNameWithoutExtension(packageArchivePath);
            string destinationPath = $"{destinationFolderPath}/{archiveName}";
            UnZip(packageArchivePath, destinationPath);
            return destinationPath;
        }
        
        private void UnZip(string packageArchivePath, string destinationPath)
        {
            Debug.Log($"UnZip from '{packageArchivePath}' to '{destinationPath}'");
            
            bool exists = File.Exists(packageArchivePath);
            if (!exists)
                throw new Exception($"File doesn't exist: {packageArchivePath}");
            
            ZipFile.ExtractToDirectory(packageArchivePath, destinationPath, true);
        }
        
        #endregion
        
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