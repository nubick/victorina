using System.Linq;
using Injection;
using MLAPI;
using UnityEngine;

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
            Player.SendFileChunkRequestToMaster(fileId, chunkIndex);
        }

        public void SendPlayerButton(float spentSeconds)
        {
            Player.SendPlayerButtonClickToMaster(spentSeconds);
        }

        public void SendSelectRoundQuestion(NetRoundQuestion netRoundQuestion)
        {
            Player.SendSelectRoundQuestionToMaster(netRoundQuestion);
        }
    }
}