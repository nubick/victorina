using System.Linq;
using Injection;
using MLAPI;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class SendToMasterService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }

        private NetworkPlayer Player
        {
            get
            {
                var networkPlayers = Object.FindObjectsOfType<NetworkPlayer>();
                NetworkPlayer networkPlayer = networkPlayers.Single(_ => _.OwnerClientId == NetworkingManager.LocalClientId);
                return networkPlayer;
            }
        }

        public void SendFileChunkRequest(int fileId, int chunkIndex)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendFileChunkRequestToMaster(fileId, chunkIndex);
            else
                Debug.Log($"Client is not connected: {nameof(SendFileChunkRequest)}");
        }

        public void SendPlayerButton(float spentSeconds)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendPlayerButtonClickToMaster(spentSeconds);
            else
                Debug.Log($"Client is not connected: {nameof(SendPlayerButton)}");
        }
        
        public void SendFilesLoadingPercentage(byte percentage, int[] downloadedFileIds)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendFilesLoadingProgressToMaster(percentage, downloadedFileIds);
            else
                Debug.Log($"Client is not connected: {nameof(SendFilesLoadingPercentage)}");
        }
        
        public void SendCommand(INetworkCommand networkCommand)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendCommand(networkCommand);
            else
                Debug.Log($"Client is not connected: {nameof(SendCommand)}");
        }
    }
}