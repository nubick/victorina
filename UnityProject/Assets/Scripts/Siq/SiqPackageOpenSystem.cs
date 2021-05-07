using System;
using System.IO;
using System.IO.Compression;
using Injection;
using UnityEngine;
using SFB;

namespace Victorina
{
    public class SiqPackageOpenSystem
    {
        [Inject] private PathData PathData { get; set; }

        public string GetPathUsingOpenDialogue()
        {
            ExtensionFilter[] extensions = {new ExtensionFilter("SIQ Files", "siq")};
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            return paths[0];
        }
        
        
#if UNITY_ANDROID
        public string Open(string path)
        {
            Debug.LogWarning("Not supported for Android.");
            return string.Empty;
        }

        public void UnZipPackageToPlayFolder(string packagePath)
        {
            Debug.LogWarning($"Not supported for Android");
        }
        
        public void UnZipPackageToEditorFolder(string packagePath)
        {
            Debug.LogWarning("Not supported for Android.");
        }

        public string GetPackageName(string packagePath)
        {
            Debug.LogWarning("Not supported for Android");
            return string.Empty;
        }
#else

        public void UnZipPackageToPlayFolder(string packagePath)
        {
            UnZip(packagePath, PathData.PackagesPath);
        }

        public void UnZipPackageToEditorFolder(string packagePath)
        {
            UnZip(packagePath, PathData.PackageEditorPath);
        }

        public string GetPackageName(string packagePath)
        {
            return Path.GetFileNameWithoutExtension(packagePath);
        }

        private void UnZip(string packagePath, string destinationPath)
        {
            Debug.Log($"Open siq pack by path: {packagePath}");
            bool exists = File.Exists(packagePath);
            if (!exists)
                throw new Exception($"File doesn't exist: {packagePath}");
            
            string packageName = Path.GetFileNameWithoutExtension(packagePath);
            string destinationDirectoryName = $"{destinationPath}/{packageName}";
            Debug.Log($"Unzip from '{packagePath}' to '{destinationDirectoryName}'");
            ZipFile.ExtractToDirectory(packagePath, destinationDirectoryName, true);
        }
#endif
    }
}