using System;
using System.Linq;
using Injection;
using MLAPI;
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
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        
        public void Initialize()
        {
            NetworkingManager.OnServerStarted += OnServerStarted;
            NetworkingManager.ConnectionApprovalCallback += OnConnectionApprovalCallback;
            NetworkingManager.OnClientConnectedCallback += OnClientConnected;
            NetworkingManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        public void StartServer()
        {
            Debug.Log($"Master: StartServer, IP: {ExternalIpData.Ip}");
            UnetTransport transport = NetworkingManager.GetComponent<UnetTransport>();
            transport.ConnectAddress = ExternalIpData.Ip;
            transport.ServerListenPort = Static.Port;
            
            NetworkingManager.StartServer();
        }
        
        private void OnServerStarted()
        {
            Debug.Log("Master: Server started");
            NetworkData.IsMaster = true;
            ConnectedPlayersData.Clear();
            MetagameEvents.ServerStarted.Publish();
        }
        
        private void OnConnectionApprovalCallback(byte[] connectionData, ulong clientId, NetworkingManager.ConnectionApprovedDelegate callback)
        {
            Debug.Log($"Master: OnConnectionApproval, clientId: {clientId}");
            ConnectionMessage connectionMessage = ConnectionMessage.FromBytes(connectionData);
            Debug.Log($"PlayerName: {connectionMessage.Name}, guid: {connectionMessage.Guid}");
            RegisterPlayer(clientId, connectionMessage);
            ulong? prefabHash = SpawnManager.GetPrefabHashFromGenerator("NetworkPlayer");
            callback(createPlayerObject: true, prefabHash, approved: true, Vector3.zero, Quaternion.identity);
        }

        private void RegisterPlayer(ulong clientId, ConnectionMessage connectionMessage)
        {
            JoinedPlayer player = ConnectedPlayersData.Players.SingleOrDefault(_ => _.ConnectionMessage.Guid == connectionMessage.Guid);
            if (player == null)
            {
                player = new JoinedPlayer();
                player.PlayerId = ConnectedPlayersData.NextPlayerId;
                ConnectedPlayersData.NextPlayerId++;
                ConnectedPlayersData.Players.Add(player);
            }
            player.ConnectionMessage = connectionMessage;
            player.ClientId = clientId;
        }
        
        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"{(NetworkData.IsMaster ? "Master" : "Client")}: OnClientConnected, clientId: {clientId}, connected clients: {GetConnectedClients()}");
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

            MetagameEvents.NetworkPlayerSpawned.Publish(networkPlayer);
            
            if (NetworkData.IsMaster)
            {
                JoinedPlayer joinedPlayer = ConnectedPlayersData.Players.Single(_ => _.ClientId == clientId);
                joinedPlayer.IsConnected = true;
                MetagameEvents.MasterClientConnected.Publish(joinedPlayer.PlayerId);
                SendToPlayersService.SendAll(networkPlayer);
            }
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (NetworkData.IsMaster)
            {
                Debug.Log($"Master. OnClientDisconnect, clientId: {clientId}, connected clients: {GetConnectedClients()}");
                JoinedPlayer joinedPlayer = ConnectedPlayersData.Players.Single(_ => _.ClientId == clientId);
                joinedPlayer.ClientId = 0;
                joinedPlayer.IsConnected = false;
                MetagameEvents.MasterClientDisconnected.Publish();
            }
        }
        
        private string GetConnectedClients()
        {
            return $"({string.Join(",", NetworkingManager.ConnectedClients.Values.Select(_ => _.ClientId))})";
        }

        public void StopServer()
        {
            if (NetworkingManager.IsServer)
            {
                Debug.Log("Server. StopServer");
                NetworkingManager.StopServer();
                ConnectedPlayersData.Clear();
                NetworkData.IsMaster = false;
                MetagameEvents.ServerStopped.Publish();
            }
        }
        
        public bool IsPlayerNameValid(string playerName)
        {
            return !string.IsNullOrWhiteSpace(playerName);
        }

    }
}