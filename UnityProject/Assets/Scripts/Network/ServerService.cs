using System.Collections.Generic;
using System.Text;
using Injection;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Spawning;
using MLAPI.Transports.UNET;
using UnityEngine;

namespace Victorina
{
    public class ServerService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private ExternalIpData ExternalIpData { get; set; }
        
        public void Initialize()
        {
            NetworkingManager.OnServerStarted += OnServerStarted;
            NetworkingManager.ConnectionApprovalCallback += OnConnectionApprovalCallback;
            NetworkingManager.OnClientConnectedCallback += OnClientConnected;
            NetworkingManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        public void StartHost()
        {
            Debug.Log($"Master: StartServer, IP: {ExternalIpData.Ip}");
            UnetTransport transport = NetworkingManager.GetComponent<UnetTransport>();
            transport.ConnectAddress = ExternalIpData.Ip;
            transport.ConnectPort = Static.Port;
            NetworkingManager.StartServer();
            NetworkData.IsAdmin = true;
        }
        
        private void OnServerStarted()
        {
            Debug.Log("OnServerStarted");
        }

        private Dictionary<ulong, string> _namesMap = new Dictionary<ulong, string>();
        
        private void OnConnectionApprovalCallback(byte[] connectionData, ulong clientId, NetworkingManager.ConnectionApprovedDelegate callback)
        {
            Debug.Log($"Server.OnConnectionApproval, clientId: {clientId}");

            string playerName = Encoding.UTF32.GetString(connectionData);
            Debug.Log($"PlayerName: {playerName}");

            _namesMap[clientId] = playerName;
            
            ulong? prefabHash = SpawnManager.GetPrefabHashFromGenerator("NetworkPlayer");

            callback(createPlayerObject: true, prefabHash, approved: true, Vector3.zero, Quaternion.identity);
        }

        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"OnClientConnected, clientId: {clientId}, connected clients: {GetConnectedClients()}");
            NetworkedObject networkedObject = SpawnManager.GetPlayerObject(clientId);
            if (networkedObject == null)
            {
                Debug.Log($"Can't get spawned object by id {clientId}");
                return;
            }

            NetworkPlayer networkPlayer = networkedObject.GetComponent<NetworkPlayer>();
            if (networkPlayer == null)
            {
                Debug.Log("Can't get NetworkPlayer component from spawned NetworkedObject");
                return;
            }

            if (NetworkingManager.IsServer)
            {
                Debug.Log($"Server. Set player name: {_namesMap[clientId]}");
                networkPlayer.SetPlayerName(_namesMap[clientId]);
            }

            MetagameEvents.PlayerConnected.Publish(networkPlayer);
        }

        private void OnClientDisconnect(ulong clientId)
        {
            Debug.Log($"OnClientDisconnect, clientId: {clientId}, connected clients: {GetConnectedClients()}");
            MetagameEvents.PlayerDisconnect.Publish(clientId);
        }

        private string GetConnectedClients()
        {
            string str = string.Empty;
            foreach (NetworkedClient client in NetworkingManager.ConnectedClients.Values)
            {
                str += $"clientId: {client.ClientId}, ";
            }
            return str;
        }

    }
}