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
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }

        public GameObject NoVideoImage;
        public VideoPlayer VideoPlayer;

        protected override void OnShown()
        {
            if (MatchData.CurrentStoryDot is VideoStoryDot videoStoryDot)
            {
                bool exists = MasterFilesRepository.Has(videoStoryDot.FileId);
                VideoPlayer.gameObject.SetActive(exists);
                NoVideoImage.SetActive(!exists);

                string path = MasterFilesRepository.GetPath(videoStoryDot.FileId);
                
                if(exists)
                {
                    string tempVideoPath = MasterFilesRepository.GetTempVideoFilePath();
                    File.Copy(path, tempVideoPath, overwrite: true);

                    string requestPath = $"file://{tempVideoPath}";
                    Debug.Log($"Request video path: '{tempVideoPath}'");
                    VideoPlayer.url = requestPath;
                    VideoPlayer.Play();
                }
                else
                {
                    Debug.Log($"Can't play video. File doesn't exist: '{path}'");
                }
            }
        }

        public void OnNextButtonClicked()
        {
            MatchSystem.ShowNext();
        }
    }
}