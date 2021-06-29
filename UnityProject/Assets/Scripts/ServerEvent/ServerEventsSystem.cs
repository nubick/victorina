using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class ServerEventsSystem
    {
        [Inject] private ServerEventsData Data { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        
        public void Initialize()
        {
            Register(ServerEvents.FinalRoundStarted);
        }

        private void Register(ServerGameEvent serverEvent)
        {
            Data.RegisteredEvents.Add(serverEvent.EventId, serverEvent);
            serverEvent.Subscribe(() => OnEventPublished(serverEvent));
        }

        private void OnEventPublished(ServerGameEvent serverEvent)
        {
            if (NetworkData.IsMaster)
                Data.PendingToSendEventIds.Enqueue(serverEvent.EventId);
        }

        public void OnPlayerReceiveServerEvent(string serverEventId)
        {
            if (Data.RegisteredEvents.ContainsKey(serverEventId))
                Data.RegisteredEvents[serverEventId].Publish();
            else
                Debug.LogWarning($"Can't find server event with id: '{serverEventId}'");
        }
    }
}