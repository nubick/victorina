using System.Collections.Generic;
using System.Text;
using System.Xml;
using Injection;
using MLAPI;
using MLAPI.Spawning;
using UnityEngine;

namespace Victorina
{
    public class ServerService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }
        
        public void Initialize()
        {
            NetworkingManager.OnServerStarted += OnServerStarted;
            NetworkingManager.ConnectionApprovalCallback += OnConnectionApprovalCallback;
            NetworkingManager.OnClientConnectedCallback += OnClientConnected;
            NetworkingManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        public void StartHost()
        {
            Debug.Log("StartHost");
            NetworkingManager.StartHost();
        }

        private void OnServerStarted()
        {
            Debug.Log("OnServerStarted");
        }

        private Dictionary<ulong, string> _namesMap = new Dictionary<ulong, string>();
        
        private void OnConnectionApprovalCallback(byte[] connectionData, ulong clientId, NetworkingManager.ConnectionApprovedDelegate callback)
        {
            Debug.Log($"OnConnectionApproval, clientId: {clientId}");

            string playerName = Encoding.UTF32.GetString(connectionData);
            Debug.Log($"PlayerName: {playerName}");

            _namesMap[clientId] = playerName;
            
            ulong? prefabHash = SpawnManager.GetPrefabHashFromGenerator("NetworkPlayer");
            
            bool approved = true;
            bool createPlayerObject = true;
            callback(createPlayerObject, prefabHash, approved, Vector3.zero, Quaternion.identity);
        }

        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"OnClientConnected: {clientId}");
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
            
            networkPlayer.PlayerName = _namesMap[clientId];
            
            MetagameEvents.PlayerConnected.Publish(networkPlayer);
        }

        private void OnClientDisconnect(ulong clientId)
        {
            Debug.Log($"OnClientDisconnect: {clientId}");
            NetworkedObject networkedObject = SpawnManager.GetPlayerObject(clientId);

            if (networkedObject == null)
            {
                Debug.Log($"NetworkedObject is null");
                return;
            }
            
            MetagameEvents.PlayerDisconnect.Publish(null);
        }
    }
}