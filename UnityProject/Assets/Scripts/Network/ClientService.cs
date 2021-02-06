using System;
using System.Collections;
using System.Text;
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
            
            byte[] playerNameBytes = Encoding.UTF32.GetBytes(playerName);
            NetworkingManager.NetworkConfig.ConnectionData = playerNameBytes;

            NetworkData.ClientConnectingState = ClientConnectingState.Connecting;
            NetworkingManager.StartClient();
            
            while (NetworkData.ClientConnectingState == ClientConnectingState.Connecting)
                yield return null;
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
            
            Debug.Log($"OnClientDisconnect: {clientId}");
            NetworkData.ClientConnectingState = ClientConnectingState.Fail;
        }
    }
}