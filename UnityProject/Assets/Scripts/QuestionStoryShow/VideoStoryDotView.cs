using System.Collections;
using Assets.Scripts.Utils;
using Injection;
using UnityEngine;
using UnityEngine.Video;

namespace Victorina
{
    public class VideoStoryDotView : StoryDotView
    {
        private int? _pendingFileId;
        
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }
        [Inject] private AppState AppState { get; set; }
        [Inject] private PathData PathData { get; set; }
        
        public GameObject NoVideoImage;
        public VideoPlayer VideoPlayer;
        public RectTransform RawImageParent;
        public RectTransform RawImage;

        public void Initialize()
        {
            MetagameEvents.ClientFileDownloaded.Subscribe(OnClientFileDownloaded);
            MetagameEvents.QuestionTimerStarted.Subscribe(OnQuestionTimeStarted);
            MetagameEvents.QuestionTimerPaused.Subscribe(OnQuestionTimerPaused);
            MetagameEvents.MediaRestarted.Subscribe(OnMediaRestarted);
            
            AppState.Volume.SubscribeChanged(SetVolume);
            SetVolume(AppState.Volume.Value);
            VideoPlayer.errorReceived += VideoPlayerOnErrorReceived;
        }

        private void VideoPlayerOnErrorReceived(VideoPlayer source, string message)
        {
            Debug.LogWarning($"VideoPlayerOnErrorReceived: {message}");
        }
        
        protected override void OnShown()
        {
            if (PlayStateData.Type == PlayStateType.AcceptingAnswer)
                return;

            StoryDot currentStoryDot = GetCurrentStoryDot();
            if (currentStoryDot is VideoStoryDot videoStoryDot)
            {
                StopAllCoroutines();
                PlayVideo(videoStoryDot.FileId);
            }
        }

        private void PlayVideo(int fileId)
        {
            StopAllCoroutines();
            StartCoroutine(PlayVideoCoroutine(fileId));
        }
        
        private IEnumerator PlayVideoCoroutine(int fileId)
        {
            bool exists = MasterFilesRepository.Has(fileId);
            VideoPlayer.gameObject.SetActive(exists);
            NoVideoImage.SetActive(!exists);
            RawImageParent.gameObject.SetActive(false);
            
            if (exists)
            {
                _pendingFileId = null;
                MasterFilesRepository.CopyToTempVideoFilePath(fileId);
                string requestPath = $"file://{PathData.TempVideoFilePath}";
                Debug.Log($"Request video path: '{PathData.TempVideoFilePath}'");
                VideoPlayer.url = requestPath;
                VideoPlayer.Prepare();

                while (!VideoPlayer.isPrepared)
                        yield return null;
                
                RefreshRawImageSize(VideoPlayer.width, VideoPlayer.height);
                VideoPlayer.Play();

                //to rid of frame from previous video
                while (VideoPlayer.frame < 0)
                    yield return null;

                RawImageParent.gameObject.SetActive(true);
            }
            else
            {
                _pendingFileId = fileId;
                Debug.Log($"Can't play video. File doesn't exist: {fileId}");
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

        private void OnRectTransformDimensionsChange()
        {
            if(IsActive && VideoPlayer.isPrepared)
                RefreshRawImageSize(VideoPlayer.width, VideoPlayer.height);
        }

        private void RefreshRawImageSize(uint videoWidth, uint videoHeight)
        {
            float parentHeight = RawImageParent.GetHeight();
            float parentWidth = RawImageParent.GetWidth();
            float imageWidth, imageHeight;
            if (videoWidth * 1f / videoHeight > parentWidth / parentHeight)
            {
                imageWidth = parentWidth;
                imageHeight = parentWidth / videoWidth * videoHeight;
            }
            else
            {
                imageHeight = parentHeight;
                imageWidth = parentHeight / videoHeight * videoWidth;
            }
            RawImage.SetWidthHeight(imageWidth, imageHeight);
        }
    }
}