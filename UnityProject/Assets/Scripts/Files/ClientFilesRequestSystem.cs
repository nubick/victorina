using System.Collections;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class ClientFilesRequestSystem
    {
        [Inject] private ClientFilesRepository ClientFilesRepository { get; set; }
        [Inject] private SendToMasterService SendToMasterService { get; set; }

        public IEnumerator RequestCoroutine()
        {
            WaitForSeconds delay = new WaitForSeconds(0.25f);
            for (;;)
            {
                yield return delay;

                DownloadingFile downloadingFile = ClientFilesRepository.Files.Values.FirstOrDefault(file => !file.IsFull());
                if (downloadingFile != null)
                {
                    int chunkIndex = downloadingFile.GetFirstEmptyChunkIndex();
                    SendToMasterService.SendFileChunkRequest(downloadingFile.FileId, chunkIndex);
                }
            }
        }
    }
}