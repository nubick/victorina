using System.Collections;
using Injection;
using UnityEngine;
using UnityEngine.Networking;

namespace Victorina
{
    public class ExternalIpSystem
    {
        [Inject] private ExternalIpData Data { get; set; }
        
        public IEnumerator Initialize()
        {
            Data.IsRequestFinished = false;
            
            UnityWebRequest unityWebRequest = UnityWebRequest.Get("https://api.ipify.org");

            yield return unityWebRequest.SendWebRequest();

            Debug.Log($"Get external ip result: {unityWebRequest.result}");
            if (unityWebRequest.result == UnityWebRequest.Result.Success)
            {
                Data.Ip = unityWebRequest.downloadHandler.text;
                Debug.Log($"External Ip: {Data.Ip}");
            }
            else
            {
                Debug.Log($"Get external ip error: {unityWebRequest.error}");
            }

            Data.HasError = unityWebRequest.result != UnityWebRequest.Result.Success;
            Data.IsRequestFinished = true;
        }
    }
}