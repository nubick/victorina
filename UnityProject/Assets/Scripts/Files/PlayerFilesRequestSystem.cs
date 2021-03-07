using System.Collections;
using System.Linq;
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
            MetagameEvents.ClientFileDownloaded.Subscribe(OnFileDownloaded);
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
                
                DownloadingFile downloadingFile = PlayerFilesRepository.Files.Values.FirstOrDefault(file => !file.IsDownloaded());
                if (downloadingFile != null)
                {
                    for (int chunkIndex = 0; chunkIndex < downloadingFile.Chunks.Length; chunkIndex++)
                    {
                        if (!Data.IsRequesting)
                            break;
                        
                        DownloadingFileChunk chunk = downloadingFile.Chunks[chunkIndex];
                        
                        if(chunk.IsDownloaded)
                            continue;
                        
                        if(chunk.RequestTime + 10f >= Time.time)
                            continue;
                        
                        SendToMasterService.SendFileChunkRequest(downloadingFile.FileId, chunkIndex);
                        chunk.RequestTime = Time.time;
                        MetagameEvents.ClientFileRequested.Publish();
                        
                        yield return delay;
                    }
                }
            }
        }

        private void OnFileDownloaded(int fileId)
        {
            var progress = PlayerFilesRepository.GetDownloadingProgress();
            byte percentage = (byte) (progress.Downloaded * 100f / progress.Total);
            SendToMasterService.SendFilesLoadingPercentage(percentage);
        }
    }
}