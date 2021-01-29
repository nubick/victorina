using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Victorina
{
    public class ClientFilesRepository : FilesRepository
    {
        public Dictionary<int, DownloadingFile> Files { get; } = new Dictionary<int, DownloadingFile>();

        public void Register(int fileId, int chunksAmount)
        {
            if (Files.ContainsKey(fileId))
            {
                Debug.Log($"Client file [{fileId}] was registered before.");
            }
            else
            {
                DownloadingFile file = new DownloadingFile(fileId, chunksAmount);
                Files.Add(fileId, file);
                Debug.Log($"Register client file [{fileId};{chunksAmount}], total registered: {Files.Count}");
            }
        }

        public void AddChunk(int fileId, int chunkIndex,  byte[] bytes)
        {
            if (Files.ContainsKey(fileId))
            {
                DownloadingFile file = Files[fileId];
                file.SetChunk(chunkIndex, bytes);

                if (file.IsFull())
                    SaveFullFile(file);
            }
            else
            {
                Debug.LogWarning($"Client repository got not registered file chunk [{fileId};{chunkIndex}]");
            }
        }

        private void SaveFullFile(DownloadingFile file)
        {
            EnsurePackageFilesDirectory();
            string path = GetPath(file.FileId);
            byte[] bytes = file.GetBytes();
            File.WriteAllBytes(path, bytes);
            MetagameEvents.ClientFileDownloaded.Publish(file.FileId);
        }
    }
}