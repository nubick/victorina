using System.Collections;
using Injection;
using UnityEngine;
using UnityEngine.Networking;

namespace Victorina
{
    public class AudioStoryDotView : StoryDotView
    {
        private int? _pendingFileId;

        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }
        [Inject] private AppState AppState { get; set; }
        [Inject] private PathSystem PathSystem { get; set; }

        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();
        
        public AudioSource AudioSource;
        
        public void Initialize()
        {
            MetagameEvents.ClientFileDownloaded.Subscribe(OnClientFileDownloaded);
            MetagameEvents.QuestionTimerStarted.Subscribe(OnQuestionTimerStarted);
            MetagameEvents.QuestionTimerPaused.Subscribe(OnQuestionTimerPaused);
            MetagameEvents.MediaRestarted.Subscribe(OnMediaRestarted);
            
            AppState.Volume.SubscribeChanged(SetVolume);
            SetVolume(AppState.Volume.Value);
        }

        private void SetVolume(float volume)
        {
            AudioSource.volume = volume;
        }

        protected override void OnShown()
        {
            if (PlayStateData.Type == PlayStateType.AcceptingAnswer)
                return;

            if (PlayState.IsCameBackFromAcceptingAnswer)
            {
                Debug.Log($"Came back from accepting answer");
                AudioSource.UnPause();
            }
            else
            {
                StoryDot currentStoryDot = GetCurrentStoryDot();
                if (currentStoryDot is AudioStoryDot audioStoryDot)
                    StartCoroutine(LoadAndPlay(audioStoryDot.FileId));
            }
        }

        private IEnumerator LoadAndPlay(int fileId)
        {
            bool exist = MasterFilesRepository.Has(fileId);
            string path = PathSystem.GetPath(fileId);
            
            if (exist)
            {
                _pendingFileId = null;
                yield return PlayAudioByPath(AudioSource, path);
            }
            else
            {
                _pendingFileId = fileId;
                Debug.Log($"Can't play audio. File doesn't exist: '{path}'");
            }
        }

        public static IEnumerator PlayAudioByPath(AudioSource audioSource, string path)
        {
            string requestPath = $"file://{path}";
            Debug.Log($"Request audio path: '{requestPath}'");
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(requestPath, AudioType.MPEG);

            yield return request.SendWebRequest();

            if (request.error != null)
            {
                Debug.Log($"Audio file loading error: {request.error}");
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
        
        private void OnClientFileDownloaded(int fileId)
        {
            if (!IsActive)
                return;

            if (_pendingFileId == fileId)
                StartCoroutine(LoadAndPlay(fileId));
        }
        
        private void OnQuestionTimerStarted()
        {
            if (IsActive)
            {
                Debug.Log($"AudioView: TimerStarted, isPlaying: {AudioSource.isPlaying}, playback pos: {AudioSource.time}, {Time.time}");
                AudioSource.UnPause();
            }
        }

        private void OnQuestionTimerPaused()
        {
            if (IsActive)
            {
                Debug.Log($"AudioView: TimerPaused, isPlaying: {AudioSource.isPlaying}, playback pos: {AudioSource.time}, {Time.time}");
                AudioSource.Pause();
            }
        }

        private void OnMediaRestarted()
        {
            if (IsActive)
            {
                AudioSource.Stop();
                AudioSource.Play();
            }
        }
    }
}