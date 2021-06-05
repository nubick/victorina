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
            if (NetworkingManager.IsConnectedClient)
                Player.SendFileChunkRequestToMaster(fileId, chunkIndex);
            else
                Debug.Log("Client is not connected: SendFileChunkRequest");
        }

        public void SendPlayerButton(float spentSeconds)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendPlayerButtonClickToMaster(spentSeconds);
            else
                Debug.Log("Client is not connected: SendPlayerButton");
        }

        public void SendSelectRoundQuestion(NetRoundQuestion netRoundQuestion)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendSelectRoundQuestionToMaster(netRoundQuestion);
            else
                Debug.Log("Client is not connected: SendSelectRoundQuestion");
        }

        public void SendFilesLoadingPercentage(byte percentage, int[] downloadedFileIds)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendFilesLoadingProgressToMaster(percentage, downloadedFileIds);
            else
                Debug.Log("Client is not connected: SendFilesLoadingPercentage");
        }

        public void SendWhoWillGetCatInBag(byte playerId)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendWhoWillGetCatInBag(playerId);
            else
                Debug.Log("Client is not connected: SendWhoWillGetCatInBag");
        }
        
        #region Auction

        public void SendPassAuction()
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendPassAuction();
            else
                Debug.Log("Client is not connected: SendPassAuction");
        }

        public void SendAllInAuction()
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendAllInAuction();
            else
                Debug.Log("Client is not connected: SendAllInAuction");
        }

        public void SendBetAuction(int bet)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendBetAuction(bet);
            else
                Debug.Log($"Client is not connected: SendBetAuction");
        }

        #endregion
        
        #region Final Round

        public void SendRemoveTheme(int index)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendRemoveTheme(index);
            else
                Debug.Log($"Client is not connected: SendRemoveTheme: {index}");
        }
        
        #endregion
    }
}