using System.Collections;
using Injection;
using UnityEngine;
using UnityEngine.Networking;

namespace Victorina
{
    public class AudioStoryDotView : ViewBase
    {
        private int? _pendingFileId;

        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }
        [Inject] private AppState AppState { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private PathSystem PathSystem { get; set; }

        public AudioSource AudioSource;
        
        public void Initialize()
        {
            MetagameEvents.ClientFileDownloaded.Subscribe(OnClientFileDownloaded);
            MetagameEvents.QuestionTimerStarted.Subscribe(OnQuestionTimerStarted);
            MetagameEvents.QuestionTimerPaused.Subscribe(OnQuestionTimerPaused);
            
            AppState.Volume.SubscribeChanged(SetVolume);
            SetVolume(AppState.Volume.Value);
        }

        private void SetVolume(float volume)
        {
            AudioSource.volume = volume;
        }
        
        protected override void OnShown()
        {
            if (MatchData.QuestionAnswerData.CurrentStoryDot is AudioStoryDot audioStoryDot)
            {
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
                if (QuestionAnswerData.MasterIntention == MasterIntention.RestartMedia)
                    AudioSource.Play();
                else
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
    }
}