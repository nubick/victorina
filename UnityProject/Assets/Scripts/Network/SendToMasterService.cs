using System.Linq;
using Injection;
using MLAPI;
using UnityEngine;

namespace Victorina
{
    public class SendToMasterService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }
        
        public void SendFileChunkRequest(int fileId, int chunkIndex)
        {
            var networkPlayers = Object.FindObjectsOfType<NetworkPlayer>();
            NetworkPlayer networkPlayer = networkPlayers.Single(_ => _.OwnerClientId == NetworkingManager.LocalClientId);
            networkPlayer.SendFileChunkRequest(fileId, chunkIndex);
        }
    }
}