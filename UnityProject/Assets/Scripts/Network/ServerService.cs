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
            MetagameEvents.ServerStarted.Publish();
        }
        
        private void OnConnectionApprovalCallback(byte[] connectionData, ulong clientId, NetworkingManager.ConnectionApprovedDelegate callback)
        {
            Debug.Log($"OnConnectionApproval, clientId: {clientId}");
            ConnectionMessage connectionMessage = ConnectionMessage.FromBytes(connectionData);
            Debug.Log($"PlayerName: {connectionMessage.Name}, guid: {connectionMessage.Guid}");
            ConnectedPlayersData.PlayersIdToConnectionMessageMap[clientId] = connectionMessage;
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
            
            if (NetworkData.IsMaster)
                ConnectedPlayersData.ConnectedClientsIds.Rewrite(NetworkingManager.ConnectedClients.Keys);
            
            MetagameEvents.PlayerConnected.Publish(networkPlayer);
            
            if(NetworkData.IsMaster)
                SendToPlayersService.SendAll(networkPlayer);
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (NetworkData.IsMaster)
            {
                Debug.Log($"OnClientDisconnect, clientId: {clientId}, connected clients: {GetConnectedClients()}");
                ConnectedPlayersData.PlayersIdToConnectionMessageMap.Remove(clientId);
                ConnectedPlayersData.ConnectedClientsIds.Rewrite(NetworkingManager.ConnectedClients.Keys);
                MetagameEvents.PlayerDisconnected.Publish();
            }
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

        public void StopServer()
        {
            if (NetworkingManager.IsServer)
            {
                Debug.Log("Server. StopServer");
                NetworkingManager.StopServer();
                NetworkData.IsMaster = false;
                MetagameEvents.ServerStopped.Publish();
            }
        }
    }
}