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
        [Inject] private AppState AppState { get; set; }
        
        public GameObject NoVideoImage;
        public VideoPlayer VideoPlayer;

        public void Initialize()
        {
            MetagameEvents.ClientFileDownloaded.Subscribe(OnClientFileDownloaded);
            MetagameEvents.QuestionTimerStarted.Subscribe(OnQuestionTimeStarted);
            MetagameEvents.QuestionTimerPaused.Subscribe(OnQuestionTimerPaused);
            MetagameEvents.MediaRestarted.Subscribe(OnMediaRestarted);
            
            AppState.Volume.SubscribeChanged(SetVolume);
            SetVolume(AppState.Volume.Value);
        }
        
        protected override void OnShown()
        {
            if (MatchData.QuestionAnswerData.CurrentStoryDot is VideoStoryDot videoStoryDot)
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

            if (exists)
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

        private void OnQuestionTimeStarted()
        {
            if (IsActive)
            {
                Debug.Log($"UnPause: VideoStoryDotView, isPaused: {VideoPlayer.isPaused}, isPlaying: {VideoPlayer.isPlaying}, isFinished: {VideoPlayer.IsFinished()}, {Time.time}");
                if (!VideoPlayer.isPlaying && !VideoPlayer.IsFinished())
                    VideoPlayer.Play();
            }
        }

        private void OnQuestionTimerPaused()
        {
            if (IsActive)
            {
                Debug.Log($"Pause: VideoStoryDotView, isPaused: {VideoPlayer.isPaused}, isPlaying: {VideoPlayer.isPlaying}, isFinished: {VideoPlayer.IsFinished()}, {Time.time}");
                if (VideoPlayer.isPlaying)
                    VideoPlayer.Pause();
            }
        }
        
        private void OnMediaRestarted()
        {
            if (IsActive)
            {
                VideoPlayer.frame = 0;
                VideoPlayer.Play();
            }
        }
        
        private void SetVolume(float volume)
        {
            VideoPlayer.SetDirectAudioVolume(0, volume);
        }
    }
}