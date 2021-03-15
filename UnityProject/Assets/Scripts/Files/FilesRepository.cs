using System.IO;
using UnityEngine;

namespace Victorina
{
    public abstract class FilesRepository
    {
        private static string _folderPrefix;

        private string GetPackageFilesPath()
        {
#if UNITY_EDITOR
            return $"{Static.DataPath}/PackageFiles";
#endif

            if (Static.BuildMode == BuildMode.Development)
            {
                if (_folderPrefix == null)
                {
                    int randomNumber = Random.Range(100, 1000);
                    //int randomNumber = 1;
                    _folderPrefix = $"test{randomNumber}";
                }

                return $"{Static.DataPath}/{_folderPrefix}/PackageFiles";
            }
            else
            {
                return $"{Static.DataPath}/PackageFiles";
            }
        }

        public string GetPath(int fileId)
        {
            return $"{GetPackageFilesPath()}/{fileId.ToString()}.dat";
        }

        public string GetTempVideoFilePath()
        {
            return $"{GetPackageFilesPath()}/TempVideo.mp4";
        }
        
        protected void EnsurePackageFilesDirectory()
        {
            string directoryPath = GetPackageFilesPath();
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
    }
}