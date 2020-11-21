using System.Text;
using UnityEngine;
using MLAPI;
using MLAPI.Spawning;

namespace Victorina
{
    public class StartupView : ViewBase
    {
        public void OnCreateNewGameButtonClicked()
        {
            StartHost();    
        }

        public void OnJoinGameButtonClicked()
        {
            JoinGame();
        }

        private void StartHost()
        {
            NetworkingManager networkingManager = NetworkingManager.Singleton;
            networkingManager.ConnectionApprovalCallback += OnConnectionApprovalCallback;
            networkingManager.StartHost();
            Debug.Log("Host started");
        }

        private void OnConnectionApprovalCallback(byte[] connectionData, ulong clientId, NetworkingManager.ConnectionApprovedDelegate callback)
        {
            Debug.Log("OnConnectionApproval");
            Debug.Log($"clientId: {clientId}");

            ulong? prefabHash = SpawnManager.GetPrefabHashFromGenerator("NetworkPlayer");

            bool approved = true;
            bool createPlayerObject = true;
            callback(createPlayerObject, prefabHash, approved, Vector3.zero, Quaternion.identity);
        }

        private void JoinGame()
        {
            byte[] login = Encoding.ASCII.GetBytes("nubick");
            NetworkingManager.Singleton.NetworkConfig.ConnectionData = login;
            NetworkingManager.Singleton.StartClient();
        }
    }
}