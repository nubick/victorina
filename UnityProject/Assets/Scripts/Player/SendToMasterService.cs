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

        public void SendWhoWillGetCatInBag(byte playerId)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendWhoWillGetCatInBag(playerId);
            else
                Debug.Log($"Client is not connected: {nameof(SendWhoWillGetCatInBag)}");
        }
        
        #region Auction

        public void SendPassAuction()
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendPassAuction();
            else
                Debug.Log($"Client is not connected: {nameof(SendPassAuction)}");
        }

        public void SendAllInAuction()
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendAllInAuction();
            else
                Debug.Log($"Client is not connected: {nameof(SendAllInAuction)}");
        }

        public void SendBetAuction(int bet)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendBetAuction(bet);
            else
                Debug.Log($"Client is not connected: {nameof(SendBetAuction)}");
        }

        #endregion
        
        #region Final Round

        public void SendRemoveTheme(int index)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendRemoveTheme(index);
            else
                Debug.Log($"Client is not connected: {nameof(SendRemoveTheme)}: {index}");
        }

        public void SendFinalRoundBet(int bet)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendFinalRoundBet(bet);
            else
                Debug.Log($"Client is not connected: {nameof(SendFinalRoundBet)}: {bet}");
        }

        #endregion

        public void SendCommand(CommandBase command)
        {
            if (NetworkingManager.IsConnectedClient)
                Player.SendCommand(command);
            else
                Debug.Log($"Client is not connected: {nameof(SendCommand)}");
        }
    }
}