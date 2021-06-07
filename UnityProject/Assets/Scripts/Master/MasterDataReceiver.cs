using System.Collections.Generic;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class MasterDataReceiver
    {
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private CatInBagSystem CatInBagSystem { get; set; }
        [Inject] private FilesDeliveryStatusManager FilesDeliveryStatusManager { get; set; }
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        
        public void OnPlayerButtonClickReceived(ulong clientId, float spentSeconds)
        {
            byte playerId = ConnectedPlayersData.GetPlayerId(clientId);
            QuestionAnswerSystem.OnPlayerButtonClickReceived(playerId, spentSeconds);
        }
        
        public void OnCurrentPlayerSelectRoundQuestionReceived(ulong clientId, NetRoundQuestion receivedNetRoundQuestion)
        {
            byte playerId = ConnectedPlayersData.GetPlayerId(clientId);
            if (!MatchSystem.IsCurrentPlayer(playerId))
            {
                Debug.Log($"Master. Validation Error. Player: {playerId} is not current. Can't select round question: {receivedNetRoundQuestion}");
                return;
            }

            if (MatchData.Phase.Value != MatchPhase.Round)
            {
                Debug.Log($"Master. Validation Error. Player: {playerId} can't select round question in phase: {MatchData.Phase.Value}");
                return;
            }

            List<NetRoundQuestion> roundQuestions = MatchData.RoundData.Value.Themes.SelectMany(theme => theme.Questions).ToList();
            NetRoundQuestion netRoundQuestion = roundQuestions.SingleOrDefault(_ => _.QuestionId == receivedNetRoundQuestion.QuestionId);

            if (netRoundQuestion == null)
            {
                Debug.Log($"Master. Validation Error. Can't find round question with id: {receivedNetRoundQuestion.QuestionId}");
                return;
            }

            if (netRoundQuestion.IsAnswered)
            {
                Debug.Log($"Master. Validation Error. Can't select answered question: {netRoundQuestion}");
                return;
            }
            
            MatchSystem.SelectQuestion(netRoundQuestion);
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
        
        #region Final Round

        public void OnReceiveRemoveTheme(ulong clientId, int index)
        {
            FinalRoundSystem.MasterOnReceiveRemoveTheme(GetPlayer(clientId), index);
        }

        public void OnReceiveFinalRoundBet(ulong clientId, int bet)
        {
            FinalRoundSystem.MasterOnReceiveBet(GetPlayer(clientId), bet);
        }
        
        #endregion
    }
}