using System.Collections;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayerFilesRequestSystem
    {
        [Inject] private PlayerFilesRepository PlayerFilesRepository { get; set; }
        [Inject] private SendToMasterService SendToMasterService { get; set; }

        public IEnumerator RequestCoroutine()
        {
            WaitForSeconds delay = new WaitForSeconds(0.10f);
            for (;;)
            {
                yield return delay;
                
                DownloadingFile downloadingFile = PlayerFilesRepository.Files.Values.FirstOrDefault(file => !file.IsDownloaded());
                if (downloadingFile != null)
                {
                    for (int chunkIndex = 0; chunkIndex < downloadingFile.Chunks.Length; chunkIndex++)
                    {
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
    }
}