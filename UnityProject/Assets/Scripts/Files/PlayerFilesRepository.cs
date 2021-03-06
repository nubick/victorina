using System.Collections.Generic;
using System.IO;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayerFilesRepository
    {
        [Inject] private PathSystem PathSystem { get; set; }
        
        public Dictionary<int, DownloadingFile> Files { get; } = new Dictionary<int, DownloadingFile>();

        public void Clear()
        {
            Files.Clear();
        }
        
        public void Register(int[] fileIds, int[] chunksAmounts, int[] priorities)
        {
            for (int i = 0; i < fileIds.Length; i++)
                Register(fileIds[i], chunksAmounts[i], priorities[i]);
        }

        public void Register(int fileId, int chunksAmount)
        {
            Register(fileId, chunksAmount, 1);
        }
        
        public void Register(int fileId, int chunksAmount, int priority)
        {
            if (Files.ContainsKey(fileId))
            {
                //Debug.Log($"Client file [{fileId}] was registered before.");
            }
            else
            {
                DownloadingFile file = new DownloadingFile(fileId, chunksAmount, priority);
                Files.Add(fileId, file);
                //Debug.Log($"Register client file [{fileId};{chunksAmount}], total registered: {Files.Count}");
                InitializeIfSavedToDisk(file);
            }
        }

        public void AddChunk(int fileId, int chunkIndex, byte[] bytes)
        {
            if (Files.ContainsKey(fileId))
            {
                DownloadingFile file = Files[fileId];
                if (file.IsEmpty(chunkIndex))
                {
                    file.SetChunk(chunkIndex, bytes);

                    if (file.IsDownloaded())
                        SaveDownloadedFile(file);
                }
            }
            else
            {
                Debug.LogWarning($"Client repository got not registered file chunk [{fileId};{chunkIndex}]");
            }
        }

        private void SaveDownloadedFile(DownloadingFile file)
        {
            string path = PathSystem.GetPath(file.FileId);
            byte[] bytes = file.GetBytes();
            File.WriteAllBytes(path, bytes);
            file.IsSavedToDisk = true;
            MetagameEvents.ClientFileDownloaded.Publish(file.FileId);
        }

        private void InitializeIfSavedToDisk(DownloadingFile file)
        {
            string path = PathSystem.GetPath(file.FileId);
            if (File.Exists(path))
            {
                //Debug.Log($"File [{file.FileId}] was downloaded before. Load from disk.");
                file.IsSavedToDisk = true;
            }
        }

        public (int Downloaded, int Total) GetDownloadingProgress()
        {
            int total = Files.Count;
            int downloaded = Files.Values.Count(_ => _.IsDownloaded());
            return (downloaded, total);
        }

        public int[] GetDownloadedFileIds()
        {
            return Files.Values.Where(_ => _.IsDownloaded()).Select(_ => _.FileId).ToArray();
        }

        public bool IsDownloaded(int fileId)
        {
            return Files.ContainsKey(fileId) && Files[fileId].IsDownloaded();
        }
    }
}