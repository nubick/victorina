using System.Collections;
using Injection;
using UnityEngine;
using UnityEngine.Networking;

namespace Victorina
{
    public class AudioStoryDotView : ViewBase
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }

        public AudioSource AudioSource;
        
        protected override void OnShown()
        {
            if (MatchData.CurrentStoryDot is AudioStoryDot audioStoryDot)
            {
                StartCoroutine(LoadAndPlay(audioStoryDot.FileId));
            }
        }

        private IEnumerator LoadAndPlay(int fileId)
        {
            bool exist = MasterFilesRepository.Has(fileId);
            string path = MasterFilesRepository.GetPath(fileId);
            
            if (exist)
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
                    AudioSource.clip = audioClip;
                    AudioSource.Play();
                }
            }
            else
            {
                Debug.Log($"Can't play audio. File doesn't exist: '{path}'");
            }
        }
        
        public void OnNextButtonClicked()
        {
            MatchSystem.ShowNext();
        }
    }
}