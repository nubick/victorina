using System.Collections;
using Injection;
using UnityEngine;
using UnityEngine.Networking;

namespace Victorina
{
    public class AudioStoryDotView : StoryDotView
    {
        private int? _pendingFileId;
        private float _pausedTime;

        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }
        [Inject] private AppState AppState { get; set; }
        [Inject] private PathSystem PathSystem { get; set; }
        
        public AudioSource AudioSource;
        
        public void Initialize()
        {
            MetagameEvents.ClientFileDownloaded.Subscribe(OnClientFileDownloaded);
            
            ServerEvents.PlayMedia.Subscribe(OnPlayMedia);
            ServerEvents.PauseMedia.Subscribe(OnPauseMedia);
            ServerEvents.RestartMedia.Subscribe(OnMediaRestarted);

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

            if (PlayStateData.Type == PlayStateType.ShowQuestion && PlayStateData.As<ShowQuestionPlayState>().IsCameBackFromAcceptingAnswer)
            {
                AudioSource.time = _pausedTime;
                AudioSource.Play();
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

        private void OnPlayMedia()
        {
            if (IsActive)
                AudioSource.UnPause();
        }

        private void OnPauseMedia()
        {
            if (IsActive)
            {
                _pausedTime = AudioSource.time;
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