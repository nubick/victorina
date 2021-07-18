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

            AnalyticsEvents.LoadPackage.Subscribe(packageName => SendEvent("LoadPackage", "PackageName", packageName));
            AnalyticsEvents.StartPackageGame.Subscribe(packageName => SendEvent("StartPackageGame", "PackageName", packageName));
            AnalyticsEvents.ResumePackageGame.Subscribe(packageName => SendEvent("ResumePackageGame", "PackageName", packageName));

            AnalyticsEvents.FirstRoundQuestionStart.Subscribe(roundNumber => SendEvent("FirstRoundQuestionStart", "RoundNumber", roundNumber));
            AnalyticsEvents.LastRoundQuestionStart.Subscribe(roundNumber => SendEvent("LastRoundQuestionStart", "RoundNumber", roundNumber));
            
            AnalyticsEvents.SupportMailClicked.Subscribe(() => SendEvent("SupportMailClicked"));
            AnalyticsEvents.DiscordInviteLinkClicked.Subscribe(() => SendEvent("DiscordInviteLinkClicked"));
            
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