using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class MasterDataReceiver
    {
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private CatInBagSystem CatInBagSystem { get; set; }
        [Inject] private FilesDeliveryStatusManager FilesDeliveryStatusManager { get; set; }
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public void OnPlayerButtonClickReceived(ulong clientId, float spentSeconds)
        {
            byte playerId = ConnectedPlayersData.GetPlayerId(clientId);
            QuestionAnswerSystem.OnPlayerButtonClickReceived(playerId, spentSeconds);
        }
        
        public void OnReceiveWhoWillGetCatInBag(ulong senderClientId, byte whoGetPlayerId)
        {
            byte senderPlayerId = ConnectedPlayersData.GetPlayerId(senderClientId);
            CatInBagSystem.OnMasterReceiveWhoWillGetCatInBag(senderPlayerId: senderPlayerId, whoGetPlayerId: whoGetPlayerId);
        }

        public void OnFilesLoadingPercentageReceived(ulong clientId, byte percentage, int[] downloadedFilesIds)
        {
            byte playerId = ConnectedPlayersData.GetPlayerId(clientId);
            PlayersBoardSystem.UpdateFilesLoadingPercentage(playerId, percentage);
            FilesDeliveryStatusManager.UpdateDownloadedFilesIds(playerId, downloadedFilesIds);
        }

        private PlayerData GetPlayer(ulong clientId)
        {
            byte playerId = ConnectedPlayersData.GetPlayerId(clientId);
            return PlayersBoardSystem.GetPlayer(playerId);
        }
        
        #region Auction

        public void OnReceivePassAuction(ulong clientId)
        {
            AuctionSystem.MasterOnReceivePlayerPass(GetPlayer(clientId));
        }

        public void OnReceiveAllInAuction(ulong clientId)
        {
            AuctionSystem.MasterOnReceivePlayerAllIn(GetPlayer(clientId));
        }

        public void OnReceiveBetAuction(ulong clientId, int bet)
        {
            AuctionSystem.MasterOnReceivePlayerBet(GetPlayer(clientId), bet);
        }
        
        #endregion
        
        public void OnReceiveCommand(ulong clientId, CommandBase command)
        {
            PlayerData player = GetPlayer(clientId);
            Debug.Log($"Master: Receive command from Player {player}: '{command}'");
            CommandsSystem.AddReceivedPlayerCommand(command, player);
        }
    }
}