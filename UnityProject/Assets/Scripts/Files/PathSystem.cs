using System.IO;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PathSystem
    {
        [Inject] public PathData Data { get; set; }
        
        public void Initialize()
        {
            Data.DataPath = GetDataPath();
            Data.PackagesPath = $"{Data.DataPath}/Packages";
            Data.PackageEditorPath = $"{Data.DataPath}/PackageEditor";
            Data.PackageFilesPath = GetPackageFilesPath();
            Data.TempVideoFilePath = $"{Data.PackageFilesPath}/TempVideo.mp4";
            
            EnsurePath(Data.PackagesPath);
            EnsurePath(Data.PackageFilesPath);
        }

        private void EnsurePath(string path)
        {
            if (Directory.Exists(path))
                return;

            Debug.Log($"Create directory '{path}'");
            Directory.CreateDirectory(path);
        }

        private string GetDataPath()
        {
#if UNITY_EDITOR
            return $"{Application.persistentDataPath}/Editor";
#else
            return Application.persistentDataPath;
#endif
        }
        
        private string GetPackageFilesPath()
        {
#if UNITY_EDITOR
            return $"{Data.DataPath}/PackageFiles";
#endif

            if (Static.BuildMode == BuildMode.Development)
            {
                string folderPrefix = $"test{Random.Range(100, 10000)}";
                return $"{Data.DataPath}/{folderPrefix}/PackageFiles";
            }
            else
            {
                return $"{Data.DataPath}/PackageFiles";
            }
        }
        
        public string GetPath(int fileId)
        {
            return $"{Data.PackageFilesPath}/{fileId.ToString()}.dat";
        }
    }
}