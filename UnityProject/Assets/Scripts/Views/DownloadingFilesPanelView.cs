using Injection;
using UnityEngine.UI;
using System.Linq;

namespace Victorina
{
    public class DownloadingFilesPanelView : ViewBase
    {
        [Inject] private ClientFilesRepository ClientFilesRepository { get; set; }
        
        public Text ProgressText;
        public Image ProgressStrip;

        public void Initialize()
        {
            MetagameEvents.ClientFileRequested.Subscribe(OnClientFileRequested);
        }

        private void OnClientFileRequested()
        {
            Show();
        }
        
        public void Update()
        {
            if (IsActive)
            {
                int total = ClientFilesRepository.Files.Count;
                int downloaded = ClientFilesRepository.Files.Values.Count(_ => _.IsDownloaded());
                ProgressText.text = $"{downloaded}/{total}";
                ProgressStrip.fillAmount = downloaded * 1f / total;
                
                if (total == downloaded)
                    Hide();
            }
        }
    }
}