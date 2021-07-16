using System.Collections;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class ClientReconnectionSystem
    {
        [Inject] private ClientReconnectionData Data { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private ClientService ClientService { get; set; }
        
        public void Initialize()
        {
            return;

            //subscribe to events
            MetagameEvents.ConnectedAsClient.Subscribe(OnClientConnected);
            MetagameEvents.DisconnectedAsClient.Subscribe(OnClientDisconnected);
        }

        private void OnClientConnected()
        {
            if (NetworkData.IsMaster)
                return;

            //Stop reconnection
        }

        private void OnClientDisconnected()
        {
            if (NetworkData.IsMaster)
                return;
            
            Debug.Log($"Reconnection: OnClientDisconnected");
            Data.StartCoroutine(Reconnect());
        }

        private IEnumerator Reconnect()
        {
            Debug.Log($"!!! Reconnect");
            yield return ClientService.JoinGameUsingLastNameAndCode();
            Debug.Log($"!!! Join finished");
            
            //call connect API
            //waiting connection

            yield return new WaitForSeconds(3f);
            
            //show reconnection UI
            //when timeout finished
            
            //redirect to StartUpView
        }
    }
}