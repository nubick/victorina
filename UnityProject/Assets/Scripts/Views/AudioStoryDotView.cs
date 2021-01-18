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

        public AudioSource AudioSource;
        
        protected override void OnShown()
        {
            if (MatchData.CurrentStoryDot is AudioStoryDot audioStoryDot)
            {
                StartCoroutine(LoadAndPlay(audioStoryDot.FullPath));
            }
        }

        private IEnumerator LoadAndPlay(string path)
        {
            string requestPath = $"file://{path}";
            Debug.Log($"Request path: '{requestPath}'");
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
        
        public void OnNextButtonClicked()
        {
            MatchSystem.ShowNext();
        }
    }
}