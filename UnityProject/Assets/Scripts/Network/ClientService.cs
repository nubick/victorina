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
        [Inject] private AppState AppState { get; set; }

        public void Initialize()
        {
            NetworkingManager.OnClientConnectedCallback += OnClientConnected;
            NetworkingManager.OnClientDisconnectCallback += OnClientDisconnect;
        }
        
        public IEnumerator JoinGame(string playerName, string gameCode)
        {
            string clientVersion = Static.DevSettings.GetAppVersion().ToString();
            Debug.Log($"JoinGame: '{playerName}', game code: {gameCode}, client version: {clientVersion}");
            
            NetworkData.IsMaster = false;
            NetworkData.RegisteredPlayerId = 0;
            
            string ip = IpCodeSystem.GetIp(gameCode);
            Debug.Log($"Joining IP: {ip}");

            UnetTransport transport = NetworkingManager.GetComponent<UnetTransport>();
            transport.ConnectAddress = ip;
            transport.ConnectPort = Static.Port;

            ConnectionMessage connectionMessage = new ConnectionMessage
            {
                Name = playerName,
                Guid = AppState.PlayerGuid,
                ClientVersion = clientVersion
            };
            
            NetworkingManager.NetworkConfig.ConnectionData = connectionMessage.ToBytes();

            NetworkData.ClientConnectingState = ClientConnectingState.Connecting;
            NetworkingManager.StartClient();
            while (NetworkData.ClientConnectingState == ClientConnectingState.Connecting)
                yield return null;

            NetworkData.LastPlayerName = playerName;//todo: I have this data in AppState data
            NetworkData.LastGameCode = gameCode;
            
            if(NetworkData.ClientConnectingState == ClientConnectingState.Rejected)
                NetworkingManager.StopClient();
        }

        public IEnumerator JoinGameUsingLastNameAndCode()
        {
            return JoinGame(NetworkData.LastPlayerName, NetworkData.LastGameCode);
        }
        
        public void LeaveGame()
        {
            if (NetworkingManager.IsClient)
            {
                Debug.Log($"Client. StopClient: {NetworkingManager.LocalClientId}");
                NetworkingManager.StopClient();
            }
        }
        
        private void OnClientConnected(ulong clientId)
        {
            if (NetworkingManager.IsServer)
                return;

            Debug.Log($"OnClientConnected, clientId: {clientId}");
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (NetworkingManager.IsServer)
                return;
            
            Debug.Log($"Player {NetworkingManager.LocalClientId}. OnClientDisconnect: {clientId}");
            NetworkData.ClientConnectingState = ClientConnectingState.Fail;
            MetagameEvents.DisconnectedAsClient.Publish();
        }
    }
}