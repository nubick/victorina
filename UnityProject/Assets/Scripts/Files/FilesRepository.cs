using System.IO;

namespace Victorina
{
    public abstract class FilesRepository
    {
        public string GetPath(int fileId)
        {
            return $"{Static.DataPath}/PackageFiles/{fileId.ToString()}.dat";
        }

        public string GetTempVideoFilePath()
        {
            return $"{Static.DataPath}/PackageFiles/TempVideo.mp4";
        }
        
        protected void EnsurePackageFilesDirectory()
        {
            string directoryPath = $"{Static.DataPath}/PackageFiles";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
    }
}