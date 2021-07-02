using System;
using System.Linq;
using Injection;
using MLAPI;
using MLAPI.Spawning;
using MLAPI.Transports.UNET;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class ServerService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private ExternalIpData ExternalIpData { get; set; }
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
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

            if (!TryParse(connectionData, out ConnectionMessage connectionMessage))
            {
                Debug.Log("Master: Can't parse connection data from client");
                SendNotApproveMessage(callback);
                return;
            }
            
            Debug.Log($"PlayerName: {connectionMessage.Name}, guid: {connectionMessage.Guid}, client version: {connectionMessage.ClientVersion}");
            
            if (IsAnotherConnection(connectionMessage))
            {
                Debug.Log("Master: Can't approve more than one connection from single player.");
                SendNotApproveMessage(callback);
            }
            else if (!IsSupportedVersion(connectionMessage))
            {
                SendNotApproveMessage(callback);
            }
            else
            {
                ApprovePlayer(clientId, connectionMessage);
                ulong? prefabHash = SpawnManager.GetPrefabHashFromGenerator("NetworkPlayer");
                callback(createPlayerObject: true, prefabHash, approved: true, Vector3.zero, Quaternion.identity);
            }
        }

        private void SendNotApproveMessage(NetworkingManager.ConnectionApprovedDelegate callback)
        {
            callback(createPlayerObject: false, playerPrefabHash: null, approved: false, position: null, rotation: null);
        }
        
        private bool TryParse(byte[] connectionData, out ConnectionMessage connectionMessage)
        {
            try
            {
                connectionMessage = ConnectionMessage.FromBytes(connectionData);
                return true;
            }
            catch (ArgumentException)
            {
                connectionMessage = null;
                return false;
            }
        }

        private bool IsAnotherConnection(ConnectionMessage connectionMessage)
        {
            JoinedPlayer joinedPlayer = ConnectedPlayersData.GetByGuid(connectionMessage.Guid);
            return joinedPlayer != null && NetworkingManager.ConnectedClients.Keys.Contains(joinedPlayer.ClientId);
        }

        private bool IsSupportedVersion(ConnectionMessage connectionMessage)
        {
            bool isSupported = false;
            if (Version.TryParse(connectionMessage.ClientVersion, out Version parsedVersion))
            {
                Version minSupportedVersion = Static.DevSettings.GetMinSupportedClientVersion();
                Version appVersion = Static.DevSettings.GetAppVersion();
                isSupported = parsedVersion >= minSupportedVersion && parsedVersion <= appVersion;
                if (!isSupported)
                    Debug.Log($"Master: Client version '{parsedVersion}' is not supported, min supported version: {minSupportedVersion}, app version: {appVersion}");
            }
            else
            {
                Debug.Log($"Master: Can't parse client version: {connectionMessage.ClientVersion}");
            }
            return isSupported;
        }

        private void ApprovePlayer(ulong clientId, ConnectionMessage connectionMessage)
        {
            CommandsSystem.AddNewCommand(new RegisterPlayerCommand {Guid = connectionMessage.Guid, Name = connectionMessage.Name});
            
            JoinedPlayer player = ConnectedPlayersData.GetByGuid(connectionMessage.Guid);
            player.Name = connectionMessage.Name;
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
                JoinedPlayer joinedPlayer = ConnectedPlayersData.GetByClientId(clientId);
                joinedPlayer.IsConnected = true;
                MetagameEvents.MasterClientConnected.Publish(joinedPlayer.PlayerId);
                SendToPlayersService.SendConnectionData(networkPlayer, joinedPlayer.PlayerId);
            }
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (NetworkData.IsMaster)
            {
                Debug.Log($"Master. OnClientDisconnect, clientId: {clientId}, connected clients: {GetConnectedClients()}");
                JoinedPlayer joinedPlayer = ConnectedPlayersData.GetByClientId(clientId);
                if (joinedPlayer != null)
                {
                    joinedPlayer.ClientId = 0;
                    joinedPlayer.IsConnected = false;
                    MetagameEvents.MasterClientDisconnected.Publish();
                }
                else
                {
                    Debug.Log($"Master: OnClientDisconnect, NOT CONNECTED CLIENT: {clientId}, connected clients: {GetConnectedClients()}");
                }
            }
        }
        
        private string GetConnectedClients()
        {
            return $"({string.Join(",", NetworkingManager.ConnectedClients.Keys)})";
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
    }
}