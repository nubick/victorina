using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace Victorina
{
    public class SiqPackOpenSystem
    {
        public string Open(string path)
        {
            Debug.Log($"Open siq pack by path: {path}");
            bool exists = File.Exists(path);
            Debug.Log($"File exists: {exists}");
            if (exists)
            {
                string packageName = Path.GetFileNameWithoutExtension(path);
                string destinationDirectoryName = $"{Application.persistentDataPath}/{packageName}";
                UnZip(path, destinationDirectoryName);
                return packageName;
            }
            throw new Exception($"File doesn't exist: {path}");
        }
        
        private void UnZip(string sourceFileName, string destinationDirectoryName)
        {
            Debug.Log($"Unzip from '{sourceFileName}' to '{destinationDirectoryName}'");
            ZipFile.ExtractToDirectory(sourceFileName, destinationDirectoryName, true);
        }
    }
}