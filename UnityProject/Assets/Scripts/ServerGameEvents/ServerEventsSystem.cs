using System;
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
            Register(ServerEvents.RoundQuestionSelected);
        }

        private void Register(ServerEventBase serverEvent)
        {
            Data.RegisteredEvents.Add(serverEvent.EventId, serverEvent);

            if (serverEvent is ServerEvent emptyServerEvent)
            {
                emptyServerEvent.Subscribe(() => OnEventPublished(serverEvent.EventId));
            }
            else if (serverEvent is ServerEvent<string> strArgServerEvent)
            {
                strArgServerEvent.Subscribe(strArg => OnEventPublished(serverEvent.EventId, strArg));
            }
            else if (serverEvent is ServerEvent<int> intArgServerEvent)
            {
                intArgServerEvent.Subscribe(intArg => OnEventPublished(serverEvent.EventId, intArg));
            }
            else
            {
                throw new Exception($"Not supported ServerEvent type: {serverEvent}");
            }
        }
        
        private void OnEventPublished(string eventId)
        {
            if (NetworkData.IsMaster)
            {
                Data.PendingToSendEvents.Enqueue((eventId, new ServerEventArgument()));
            }
        }

        private void OnEventPublished(string eventId, string strArgument)
        {
            if (NetworkData.IsMaster)
            {
                ServerEventArgument argument = new ServerEventArgument();
                argument.SetString(strArgument);
                Data.PendingToSendEvents.Enqueue((eventId, argument));
            }
        }

        private void OnEventPublished(string eventId, int intArgument)
        {
            if (NetworkData.IsMaster)
            {
                ServerEventArgument argument = new ServerEventArgument();
                argument.SetInt(intArgument);
                Data.PendingToSendEvents.Enqueue((eventId, argument));
            }
        }

        public void OnPlayerReceiveServerEvent(string serverEventId, ServerEventArgument argument)
        {
            if (Data.RegisteredEvents.ContainsKey(serverEventId))
            {
                ServerEventBase serverEvent = Data.RegisteredEvents[serverEventId];
                if (serverEvent is ServerEvent emptyServerEvent)
                    emptyServerEvent.Publish();
                else if (serverEvent is ServerEvent<string> stringServerEvent)
                    stringServerEvent.Publish(argument.AsString());
                else if (serverEvent is ServerEvent<int> intServerEvent)
                    intServerEvent.Publish(argument.AsInt());
            }
            else
                Debug.LogWarning($"Can't find server event with id: '{serverEventId}'");
        }
    }
}