using System;
using System.Collections;
using Injection;
using MLAPI;
using MLAPI.Transports.UNET;
using UnityEngine;

namespace Victorina
{
    public class ClientService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private IpCodeSystem IpCodeSystem { get; set; }
        [Inject] private ViewsSystem ViewsSystem { get; set; }
        [Inject] private AppState AppState { get; set; }

        public void Initialize()
        {
            NetworkingManager.OnClientConnectedCallback += OnClientConnected;
            NetworkingManager.OnClientDisconnectCallback += OnClientDisconnect;
        }
        
        public IEnumerator JoinGame(string playerName, string gameCode)
        {
            Debug.Log($"JoinGame: '{playerName}', game code: {gameCode}");

            NetworkData.IsMaster = false;
            
            string ip = IpCodeSystem.GetIp(gameCode);
            Debug.Log($"Joining IP: {ip}");

            UnetTransport transport = NetworkingManager.GetComponent<UnetTransport>();
            transport.ConnectAddress = ip;
            transport.ConnectPort = Static.Port;

            ConnectionMessage connectionMessage = new ConnectionMessage
            {
                Name = playerName,
                Guid = AppState.PlayerGuid
            };
            
            NetworkingManager.NetworkConfig.ConnectionData = connectionMessage.ToBytes();

            NetworkData.ClientConnectingState = ClientConnectingState.Connecting;
            NetworkingManager.StartClient();
            
            while (NetworkData.ClientConnectingState == ClientConnectingState.Connecting)
                yield return null;
        }

        public void LeaveGame()
        {
            if (NetworkingManager.IsClient)
            {
                Debug.Log($"Client. StopClient: {NetworkingManager.LocalClientId}");
                NetworkingManager.StopClient();
                MetagameEvents.DisconnectedAsClient.Publish();
            }
        }
        
        private void OnClientConnected(ulong clientId)
        {
            if (NetworkingManager.IsServer)
                return;
            
            if (NetworkingManager.LocalClientId == clientId)
            {
                Debug.Log($"Client connection success, clientId: {clientId}");
                NetworkData.ClientConnectingState = ClientConnectingState.Success;
                NetworkData.PlayerId = clientId;
                MetagameEvents.ConnectedAsClient.Publish();
            }
            else
            {
                NetworkData.ClientConnectingState = ClientConnectingState.Fail;
                throw new Exception($"I can see connection of another client, id: {clientId}, LocalClientId: {NetworkingManager.LocalClientId}");
            }
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (NetworkingManager.IsServer)
                return;
            
            Debug.Log($"Player. OnClientDisconnect: {clientId}");
            NetworkData.ClientConnectingState = ClientConnectingState.Fail;
            ViewsSystem.OnClientDisconnected();
            MetagameEvents.DisconnectedAsClient.Publish();
        }
    }
}