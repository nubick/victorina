using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Victorina.DevTools;


namespace Victorina
{
    public class AnalyticsSystem
    {
        public void Initialize()
        {
            MetagameEvents.ServerStarted.Subscribe(() => SendEvent("ServerStarted"));
            MetagameEvents.ServerStopped.Subscribe(() => SendEvent("ServerStopped"));
            MetagameEvents.ConnectedAsClient.Subscribe(() => SendEvent("ConnectedAsClient"));
            MetagameEvents.DisconnectedAsClient.Subscribe(() => SendEvent("DisconnectedAsClient"));

            AnalyticsEvents.LoadPackageToPlay.Subscribe(packageName => SendEvent("LoadPackageToPlay", "PackageName", packageName));

            AnalyticsEvents.FirstRoundQuestionStart.Subscribe(roundNumber => SendEvent("FirstRoundQuestionStart", "RoundNumber", roundNumber));
            AnalyticsEvents.LastRoundQuestionStart.Subscribe(roundNumber => SendEvent("LastRoundQuestionStart", "RoundNumber", roundNumber));
            
            //Crafter
            AnalyticsEvents.CrafterOpen.Subscribe(() => SendEvent("CrafterOpen"));
            AnalyticsEvents.SavePackageAsArchive.Subscribe(archiveName => SendEvent("SavePackageAsArchive", "ArchiveName", archiveName));
        }
        
        private void SendEvent(string eventName)
        {
            Dev.Log(eventName, Color.yellow);
            Analytics.CustomEvent(eventName);
        }

        private void SendEvent(string eventName, string parameterName, string parameterValue)
        {
            Dev.Log($"{eventName}|{parameterName}|{parameterValue}", Color.yellow);
            var eventData = new Dictionary<string, object> {{parameterName, parameterValue}};
            Analytics.CustomEvent(eventName, eventData);
        }
        
        private void SendEvent(string eventName, string parameterName, int parameterValue)
        {
            Dev.Log($"{eventName}|{parameterName}|{parameterValue}", Color.yellow);
            var eventData = new Dictionary<string, object> {{parameterName, parameterValue}};
            Analytics.CustomEvent(eventName, eventData);
        }

        public void OnDestroy()
        {
            Analytics.FlushEvents();
        }
    }
}