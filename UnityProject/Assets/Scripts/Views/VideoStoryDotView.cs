using System.IO;
using Injection;
using UnityEngine;
using UnityEngine.Video;

namespace Victorina
{
    public class VideoStoryDotView : ViewBase
    {
        private int? _pendingFileId;
        
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }

        public GameObject NoVideoImage;
        public VideoPlayer VideoPlayer;

        public void Initialize()
        {
            MetagameEvents.ClientFileDownloaded.Subscribe(OnClientFileDownloaded);
        }

        protected override void OnShown()
        {
            if (MatchData.CurrentStoryDot is VideoStoryDot videoStoryDot)
            {
                PlayVideo(videoStoryDot.FileId);
            }
        }

        private void PlayVideo(int fileId)
        {
            bool exists = MasterFilesRepository.Has(fileId);
            VideoPlayer.gameObject.SetActive(exists);
            NoVideoImage.SetActive(!exists);

            string path = MasterFilesRepository.GetPath(fileId);
                
            if(exists)
            {
                _pendingFileId = null;
                string tempVideoPath = MasterFilesRepository.GetTempVideoFilePath();
                File.Copy(path, tempVideoPath, overwrite: true);

                string requestPath = $"file://{tempVideoPath}";
                Debug.Log($"Request video path: '{tempVideoPath}'");
                VideoPlayer.url = requestPath;
                VideoPlayer.Play();
            }
            else
            {
                _pendingFileId = fileId;
                Debug.Log($"Can't play video. File doesn't exist: '{path}'");
            }
        }
        
        private void OnClientFileDownloaded(int fileId)
        {
            if (!IsActive)
                return;

            if (_pendingFileId == fileId)
                PlayVideo(fileId);
        }
    }
}