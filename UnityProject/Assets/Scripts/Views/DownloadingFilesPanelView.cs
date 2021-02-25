using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class DownloadingFilesPanelView : ViewBase
    {
        [Inject] private PlayerFilesRepository PlayerFilesRepository { get; set; }
        
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
                var progress = PlayerFilesRepository.GetDownloadingProgress();
                ProgressText.text = $"{progress.Downloaded}/{progress.Total}";
                ProgressStrip.fillAmount = progress.Downloaded * 1f / progress.Total;
                
                if (progress.Total == progress.Downloaded)
                    Hide();
            }
        }
    }
}