using System.Text.RegularExpressions;
using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class DownloadingFilesPanelView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PlayerFilesRepository PlayerFilesRepository { get; set; }
        
        public Text ProgressText;
        public Image ProgressStrip;

        public void Initialize()
        {
            //MetagameEvents.ClientFileRequested.Subscribe(OnClientFileRequested);
        }

        private void OnClientFileRequested()
        {
            if (!IsActive && MatchData.Phase.Value == MatchPhase.Round)
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