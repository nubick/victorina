using System.IO;
using Injection;
using UnityEngine;
using UnityEngine.Video;

namespace Victorina
{
    public class VideoStoryDotView : ViewBase
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }

        public GameObject NoVideoImage;
        public VideoPlayer VideoPlayer;

        protected override void OnShown()
        {
            if (MatchData.CurrentStoryDot is VideoStoryDot videoStoryDot)
            {
                bool exists = File.Exists(videoStoryDot.FullPath);
                VideoPlayer.gameObject.SetActive(exists);
                NoVideoImage.SetActive(!exists);
                
                if(exists)
                {
                    string requestPath = $"file://{videoStoryDot.FullPath}";
                    Debug.Log($"Request video path: '{requestPath}'");
                    VideoPlayer.url = requestPath;
                    VideoPlayer.Play();
                }
                else
                {
                    Debug.Log($"Can't play video. File doesn't exist: '{videoStoryDot.FullPath}'");
                }
            }
        }

        public void OnNextButtonClicked()
        {
            MatchSystem.ShowNext();
        }
    }
}