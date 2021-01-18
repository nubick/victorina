using Injection;
using UnityEngine;
using UnityEngine.Video;

namespace Victorina
{
    public class VideoStoryDotView : ViewBase
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
        public VideoPlayer VideoPlayer;

        protected override void OnShown()
        {
            if (MatchData.CurrentStoryDot is VideoStoryDot videoStoryDot)
            {
                string requestPath = $"file://{videoStoryDot.FullPath}";
                Debug.Log($"Request video path: '{requestPath}'");
                VideoPlayer.url = requestPath;
                VideoPlayer.Play();
            }
        }

        public void OnNextButtonClicked()
        {
            MatchSystem.ShowNext();
        }
    }
}