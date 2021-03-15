using System.Collections;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayerFilesRequestSystem
    {
        [Inject] private PlayerFilesRequestData Data { get; set; }
        [Inject] private PlayerFilesRepository PlayerFilesRepository { get; set; }
        [Inject] private SendToMasterService SendToMasterService { get; set; }
        
        public void Initialize()
        {
            MetagameEvents.ClientFileDownloaded.Subscribe(_ => SendLoadingProgress());
            MetagameEvents.ConnectedAsClient.Subscribe(StartRequesting);
            MetagameEvents.DisconnectedAsClient.Subscribe(StopRequesting);

            Data.StartCoroutine(RequestCoroutine());
        }

        private void StartRequesting()
        {
            Debug.Log("Start Requesting: PlayerFilesRequestSystem");
            PlayerFilesRepository.Clear();
            Data.IsRequesting = true;
        }

        private void StopRequesting()
        {
            Debug.Log("Stop Requesting: PlayerFilesRequestSystem");
            Data.IsRequesting = false;
            PlayerFilesRepository.Clear();
        }
        
        private IEnumerator RequestCoroutine()
        {
            WaitForSeconds delay = new WaitForSeconds(0.10f);
            for (;;)
            {
                yield return delay;
                
                if(!Data.IsRequesting)
                    continue;

                DownloadingFile downloadingFile = GetNextFile();
                if (downloadingFile != null)
                    yield return DownloadFile(downloadingFile);
            }
        }

        private DownloadingFile GetNextFile()
        {
            DownloadingFile minPriorityFile = null;
            foreach (DownloadingFile file in PlayerFilesRepository.Files.Values)
            {
                if (file.IsDownloaded())
                    continue;

                if (minPriorityFile == null || file.Priority < minPriorityFile.Priority)
                    minPriorityFile = file;
            }
            return minPriorityFile;
        }

        private IEnumerator DownloadFile(DownloadingFile file)
        {
            //Debug.Log($"Download file: {file}");
            for (int chunkIndex = 0; chunkIndex < file.Chunks.Length; chunkIndex++)
            {
                DownloadingFileChunk chunk = file.Chunks[chunkIndex];
                while (!chunk.IsDownloaded)
                {
                    if (!Data.IsRequesting)
                        yield break;
                    
                    SendToMasterService.SendFileChunkRequest(file.FileId, chunkIndex);
                    MetagameEvents.ClientFileRequested.Publish();
                    float requestTime = Time.time;
                    while (!chunk.IsDownloaded && Time.time - requestTime < 10f && Data.IsRequesting)
                        yield return null;
                }
            }
        }
        
        public void SendLoadingProgress()
        {
            var progress = PlayerFilesRepository.GetDownloadingProgress();
            byte percentage = (byte) (progress.Downloaded * 100f / progress.Total);
            int[] downloadedFileIds = PlayerFilesRepository.GetDownloadedFileIds();
            SendToMasterService.SendFilesLoadingPercentage(percentage, downloadedFileIds);
        }
    }
}