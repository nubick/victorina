using System;
using System.Linq;
using Injection;
using MLAPI;
using MLAPI.Messaging;
using Debug = UnityEngine.Debug;

namespace Victorina
{
    public class NetworkPlayer : NetworkedBehaviour
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }
        [Inject] private PlayerDataReceiver PlayerDataReceiver { get; set; }
        [Inject] private MasterDataReceiver MasterDataReceiver { get; set; }
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        
        public string GetPlayerInfo()
        {
            JoinedPlayer joinedPlayer = ConnectedPlayersData.GetByClientId(OwnerClientId);
            return joinedPlayer == null ?
                $"[Can't find by: {OwnerClientId}]" : 
                $"[{joinedPlayer.ConnectionMessage.Name}|{OwnerClientId}]";
        }

        public void SendRegisteredPlayerId(byte playerId)
        {
            Debug.Log($"Master: Send Registered PlayerId {playerId} to {GetPlayerInfo()}");
            InvokeClientRpcOnOwner(ReceiveRegisteredPlayerId, playerId, "Sequenced");
        }

        [ClientRPC]
        private void ReceiveRegisteredPlayerId(byte playerId)
        {
            Debug.Log($"Player {OwnerClientId}: Receive registered player id: {playerId}");
            PlayerDataReceiver.OnReceiveRegisteredPlayerId(playerId);
        }
        
        public void SendPlayersBoard(PlayersBoard playersBoard)
        {
            //Debug.Log($"Master: Send PlayersBoard: {playersBoard} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceivePlayersBoard, playersBoard, "Sequenced");
        }

        [ClientRPC]
        private void ReceivePlayersBoard(PlayersBoard playersBoard)
        {
            //Debug.Log($"Player {OwnerClientId}: Receive PlayersBoard: {playersBoard}");
            PlayerDataReceiver.OnReceive(playersBoard);
        }

        public void SendMatchPhase(MatchPhase matchPhase)
        {
            //Debug.Log($"Master: Send match phase: {matchPhase} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveMatchPhase, matchPhase);
        }

        [ClientRPC]
        private void ReceiveMatchPhase(MatchPhase matchPhase)
        {
            Debug.Log($"Player {OwnerClientId}: Receive match phase: {matchPhase}");
            PlayerDataReceiver.OnReceive(matchPhase);
        }
        
        public void SendNetRound(NetRound netRound)
        {
            //Debug.Log($"Master: Send RoundData: {netRound} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveRoundData, netRound, "RFS");
        }

        [ClientRPC]
        private void ReceiveRoundData(NetRound netRound)
        {
            //Debug.Log($"Player {OwnerClientId}: Receive RoundData: {netRound}");
            PlayerDataReceiver.OnReceiveNetRound(netRound);
        }

        public void SendQuestionAnswerData(QuestionAnswerData questionAnswerData)
        {
            //Debug.Log($"Master: Send question answer data: {questionAnswerData} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveQuestionAnswerData, questionAnswerData);
        }

        [ClientRPC]
        private void ReceiveQuestionAnswerData(QuestionAnswerData questionAnswerData)
        {
            Debug.Log($"Player {OwnerClientId}: Receive question answer data: {questionAnswerData}");
            PlayerDataReceiver.OnReceive(questionAnswerData);
        }

        public void SendCatInBagData(CatInBagData data)
        {
            InvokeClientRpcOnOwner(ReceiveCatInBagData, data.IsPlayerSelected.Value);
        }

        [ClientRPC]
        private void ReceiveCatInBagData(bool isPlayerSelected)
        {
            Debug.Log($"Player {OwnerClientId}: Receive cat in bag data, isPlayerSelected: {isPlayerSelected}");
            PlayerDataReceiver.OnReceiveCatInBagData(isPlayerSelected);
        }

        public void SendAuctionData(AuctionData auctionData)
        {
            InvokeClientRpcOnOwner(ReceiveAuctionData, auctionData);
        }

        [ClientRPC]
        private void ReceiveAuctionData(AuctionData auctionData)
        {
            Debug.Log($"Player {OwnerClientId}: Receive AuctionData: {auctionData}");
            PlayerDataReceiver.OnReceiveAuctionData(auctionData);
        }

        public void SendSelectedQuestion(NetQuestion netQuestion)
        {
            //Debug.Log($"Master: Send selected question: {netQuestion} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveSelectedQuestion, netQuestion);

            for (int index = 0; index < netQuestion.QuestionStory.Length; index++)
            {
                //Debug.Log($"Master: Send question story dot to All: {storyDot}");
                SendStoryDot(netQuestion.QuestionStory[index], isQuestion: true, index);
            }

            for (int index = 0; index < netQuestion.AnswerStory.Length; index++)
            {
                //Debug.Log($"Master: Send answer story dot to All: {storyDot}");
                SendStoryDot(netQuestion.AnswerStory[index], isQuestion: false, index);
            }
        }

        [ClientRPC]
        private void ReceiveSelectedQuestion(NetQuestion netQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive selected question: {netQuestion}");
            netQuestion.QuestionStory = new StoryDot[netQuestion.QuestionStoryDotsAmount];
            netQuestion.AnswerStory = new StoryDot[netQuestion.AnswerStoryDotsAmount];
            MatchData.QuestionAnswerData.SelectedQuestion.Value = netQuestion;
        }

        public void SendStoryDot(StoryDot storyDot, bool isQuestion, int index)
        {
            //Debug.Log($"Master: Send story dot: {storyDot} to {OwnerClientId}");
            if (storyDot is TextStoryDot textStoryDot)
                InvokeClientRpcOnOwner(ReceiveTextStoryDot, textStoryDot, isQuestion, index, "RFS");
            else if (storyDot is ImageStoryDot imageStoryDot)
                InvokeClientRpcOnOwner(ReceiveImageStoryDot, imageStoryDot, isQuestion, index);
            else if (storyDot is AudioStoryDot audioStoryDot)
                InvokeClientRpcOnOwner(ReceiveAudioStoryDot, audioStoryDot, isQuestion, index);
            else if (storyDot is VideoStoryDot videoStoryDot)
                InvokeClientRpcOnOwner(ReceiveVideoStoryDot, videoStoryDot, isQuestion, index);
            else if (storyDot is NoRiskStoryDot noRiskStoryDot)
                InvokeClientRpcOnOwner(ReceiveNoRiskStoryDot, noRiskStoryDot, index);
            else if (storyDot is CatInBagStoryDot catInBagStoryDot)
                InvokeClientRpcOnOwner(ReceiveCatInBagStoryDot, catInBagStoryDot, index);
            else if (storyDot is AuctionStoryDot auctionStoryDot)
                InvokeClientRpcOnOwner(ReceiveAuctionStoryDot, auctionStoryDot, index);
            else
                throw new Exception($"Not supported story dot: {storyDot}");
        }

        private void SetStoryDot(StoryDot storyDot, bool isQuestion, int index)
        {
            if (isQuestion)
                MatchData.QuestionAnswerData.SelectedQuestion.Value.QuestionStory[index] = storyDot;
            else
                MatchData.QuestionAnswerData.SelectedQuestion.Value.AnswerStory[index] = storyDot;
        }
        
        [ClientRPC]
        private void ReceiveTextStoryDot(TextStoryDot textStoryDot, bool isQuestion, int index)
        {
            Debug.Log($"Player {OwnerClientId}: {(isQuestion ? "Q" : "A")}[{index}]: Receive text story dot: {textStoryDot}");
            SetStoryDot(textStoryDot, isQuestion, index);
        }

        [ClientRPC]
        private void ReceiveImageStoryDot(ImageStoryDot imageStoryDot, bool isQuestion, int index)
        {
            Debug.Log($"Player {OwnerClientId}: {(isQuestion ? "Q" : "A")}[{index}]: Receive image story dot: {imageStoryDot}");
            SetStoryDot(imageStoryDot, isQuestion, index);
            PlayerDataReceiver.OnFileStoryDotReceived(imageStoryDot);
        }

        [ClientRPC]
        private void ReceiveAudioStoryDot(AudioStoryDot audioStoryDot, bool isQuestion, int index)
        {
            Debug.Log($"Player {OwnerClientId}: {(isQuestion ? "Q" : "A")}[{index}]: Receive audio story dot: {audioStoryDot}");
            SetStoryDot(audioStoryDot, isQuestion, index);
            PlayerDataReceiver.OnFileStoryDotReceived(audioStoryDot);
        }

        [ClientRPC]
        private void ReceiveVideoStoryDot(VideoStoryDot videoStoryDot, bool isQuestion, int index)
        {
            Debug.Log($"Player {OwnerClientId}: {(isQuestion ? "Q" : "A")}[{index}]: Receive video story dot: {videoStoryDot}");
            SetStoryDot(videoStoryDot, isQuestion, index);
            PlayerDataReceiver.OnFileStoryDotReceived(videoStoryDot);
        }

        [ClientRPC]
        private void ReceiveNoRiskStoryDot(NoRiskStoryDot noRiskStoryDot, int index)
        {
            Debug.Log($"Player {OwnerClientId}: [{index}]: Receive no risk story dot");
            SetStoryDot(noRiskStoryDot, isQuestion: true, index);
        }

        [ClientRPC]
        private void ReceiveCatInBagStoryDot(CatInBagStoryDot catInBagStoryDot, int index)
        {
            Debug.Log($"Player {OwnerClientId}: [{index}]: Receive cat in bag story dot: {catInBagStoryDot}");
            SetStoryDot(catInBagStoryDot, isQuestion: true, index);
        }

        [ClientRPC]
        private void ReceiveAuctionStoryDot(AuctionStoryDot auctionStoryDot, int index)
        {
            Debug.Log($"Player {OwnerClientId}: [{index}]: Receive auction story dot");
            SetStoryDot(auctionStoryDot, isQuestion: true, index);
        }
        
        public void SendSelectedRoundQuestion(NetRoundQuestion netRoundQuestion)
        {
            //Debug.Log($"Master: Send selected round question: {netRoundQuestion} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveSelectedRoundQuestion, netRoundQuestion);
        }

        [ClientRPC]
        private void ReceiveSelectedRoundQuestion(NetRoundQuestion netRoundQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive selected round question: {netRoundQuestion}");
            MatchData.SelectedRoundQuestion = netRoundQuestion;
        }

        public void SendNetRoundsInfo(NetRoundsInfo netRoundsInfo)
        {
            //Debug.Log($"Master: Send rounds info: {netRoundsInfo} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveNetRoundsInfo, netRoundsInfo);
        }

        [ClientRPC]
        private void ReceiveNetRoundsInfo(NetRoundsInfo netRoundsInfo)
        {
            Debug.Log($"Player {OwnerClientId}: Receive rounds info: {netRoundsInfo}");
            MatchData.RoundsInfo.Value = netRoundsInfo;
        }

        #region Files

        private void SendFileChunk(int fileId, int chunkIndex)
        {
            if (MasterFilesRepository.Has(fileId))
            {
                //Debug.Log($"Master: Send file chunk [{fileId};{chunkIndex}] to Player {OwnerClientId}");
                byte[] bytes = MasterFilesRepository.GetFileChunk(fileId, chunkIndex);
                InvokeClientRpcOnOwner(ReceiveFileChunk, fileId, chunkIndex, bytes, "RFS");
            }
            else
            {
                Debug.LogWarning($"Master: Can't send nonexistent file [{fileId}]");
            }
        }

        [ClientRPC]
        private void ReceiveFileChunk(int fileId, int chunkIndex, byte[] bytes)
        {
            //Debug.Log($"Player {OwnerClientId}: Receive file chunk [{fileId};{chunkIndex}], bytes: {bytes.SizeKb()}, ({Time.time:0.000})");
            PlayerDataReceiver.OnFileChunkReceived(fileId, chunkIndex, bytes);
        }

        public void SendFileChunkRequestToMaster(int fileId, int chunkIndex)
        {
            //Debug.Log($"Player {OwnerClientId}: Request file chunk [{fileId};{chunkIndex}], ({Time.time:0.000})");
            InvokeServerRpc(MasterReceiveClientFileRequest, fileId, chunkIndex);
        }

        [ServerRPC]
        private void MasterReceiveClientFileRequest(int fileId, int chunkIndex)
        {
            //Debug.Log($"Master: Receive Player {OwnerClientId} file chunk request: [{fileId};{chunkIndex}]");
            SendFileChunk(fileId, chunkIndex);
        }

        public void SendRoundFileIds(int[] fileIds, int[] chunksAmounts, int[] priorities)
        {
            Debug.Log($"Master: Send round of file ids ({fileIds.Length}) to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveRoundFileIds, fileIds, chunksAmounts, priorities, "RFS");
        }

        [ClientRPC]
        private void ReceiveRoundFileIds(int[] fileIds, int[] chunksAmounts, int[] priorities)
        {
            Debug.Log($"Player {OwnerClientId}: Receive round file ids, amount: {fileIds.Length}");
            PlayerDataReceiver.OnRoundFileIdsReceived(fileIds, chunksAmounts, priorities);
        }

        #endregion

        #region Timer and button

        public void SendPlayerButtonClickToMaster(float spentSeconds)
        {
            Debug.Log($"Player {OwnerClientId}: send button click to Master, thoughtSeconds: {spentSeconds}");
            InvokeServerRpc(MasterReceivePlayerButtonClick, spentSeconds);
        }

        [ServerRPC]
        private void MasterReceivePlayerButtonClick(float spentSeconds)
        {
            Debug.Log($"Master: Receive Player {GetPlayerInfo()} button click, spentSeconds: {spentSeconds}");
            MasterDataReceiver.OnPlayerButtonClickReceived(OwnerClientId, spentSeconds);
        }

        public void SendPlayersButtonClickData(PlayersButtonClickData playerButtonClickData)
        {
            //Debug.Log($"Master: Send players button click data to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceivePlayersButtonClickData, playerButtonClickData);
        }

        [ClientRPC]
        private void ReceivePlayersButtonClickData(PlayersButtonClickData playersButtonClickData)
        {
            Debug.Log($"Player {OwnerClientId}: Receive players button click data: {playersButtonClickData}");
            PlayerDataReceiver.OnReceive(playersButtonClickData);
        }

        #endregion

        #region Send Select Round Question to MASTER
        
        public void SendSelectRoundQuestionToMaster(NetRoundQuestion netRoundQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: send select round question to Master: {netRoundQuestion}");
            InvokeServerRpc(MasterReceiveSelectRoundQuestion, netRoundQuestion);
        }

        [ServerRPC]
        private void MasterReceiveSelectRoundQuestion(NetRoundQuestion netRoundQuestion)
        {
            Debug.Log($"Master: Receive select round question, Player {GetPlayerInfo()}, netRoundQuestion: {netRoundQuestion}");
            MasterDataReceiver.OnCurrentPlayerSelectRoundQuestionReceived(OwnerClientId, netRoundQuestion);
        }
        
        #endregion

        #region Files Loading Percentage to MASTER

        public void SendFilesLoadingProgressToMaster(byte percentage, int[] downloadedFileIds)
        {
            //Debug.Log($"Player {OwnerClientId}: send files loading progress to Master, percentage: {percentage}, amount: {downloadedFileIds.Length}");
            InvokeServerRpc(MasterReceiveFilesLoadingProgress, percentage, downloadedFileIds, "RFS");
        }
        
        [ServerRPC]
        private void MasterReceiveFilesLoadingProgress(byte percentage, int[] downloadedFilesIds)
        {
            //Debug.Log($"Master: Receive files loading progress, percentage: {percentage}, amount: '{downloadedFilesIds.Length}' from Player {GetPlayerInfo()}");
            MasterDataReceiver.OnFilesLoadingPercentageReceived(OwnerClientId, percentage, downloadedFilesIds);
        }
        
        #endregion
        
        #region Send Who Will Get Cat In Bag to MASTER

        public void SendWhoWillGetCatInBag(byte playerId)
        {
            Debug.Log($"Player {OwnerClientId}: send who will get cat in bag to Master: {playerId}");
            InvokeServerRpc(MasterReceiveWhoWillGetCatInBag, playerId);
        }

        [ServerRPC]
        private void MasterReceiveWhoWillGetCatInBag(byte playerId)
        {
            Debug.Log($"Master: Receive who will get cat in bag: {playerId} from Player {GetPlayerInfo()}");
            MasterDataReceiver.OnReceiveWhoWillGetCatInBag(senderClientId: OwnerClientId, whoGetPlayerId: playerId);
        }
        
        #endregion
        
        #region Effects

        public void SendPlaySoundEffectCommand(int number)
        {
            InvokeClientRpcOnOwner(ReceivePlaySoundEffectCommand, number);
        }

        [ClientRPC]
        private void ReceivePlaySoundEffectCommand(int number)
        {
            Debug.Log($"Player {OwnerClientId}: Receive play sound effect command");
            PlayerDataReceiver.OnReceivePlaySoundEffectCommand(number);
        }
        
        #endregion

        public void SendAnsweringTimerData(AnsweringTimerData data)
        {
            InvokeClientRpcOnOwner(ReceiveAnsweringTimerData, data.IsRunning, data.MaxSeconds, data.LeftSeconds);
        }

        [ClientRPC]
        private void ReceiveAnsweringTimerData(bool isRunning, float maxSeconds, float leftSeconds)
        {
            //Debug.Log($"Player {OwnerClientId}: Receive answering timer data, isRunning: {isRunning}, startSeconds: {maxSeconds}, leftSeconds: {leftSeconds}");
            PlayerDataReceiver.OnReceiveAnsweringTimerData(isRunning, maxSeconds, leftSeconds);
        }
        
        #region Auction

        public void SendPassAuction()
        {
            Debug.Log($"Player {OwnerClientId}: send pass auction to Master");
            InvokeServerRpc(MasterReceivePassAuction);
        }
        
        [ServerRPC]
        private void MasterReceivePassAuction()
        {
            Debug.Log($"Master: Receive pass auction from Player {GetPlayerInfo()}");
            MasterDataReceiver.OnReceivePassAuction(OwnerClientId);
        }

        public void SendAllInAuction()
        {
            Debug.Log($"Player {OwnerClientId}: send all-in auction to Master");
            InvokeServerRpc(MasterReceiveAllInAuction);
        }

        [ServerRPC]
        private void MasterReceiveAllInAuction()
        {
            Debug.Log($"Master: Receive all in auction from Player {GetPlayerInfo()}");
            MasterDataReceiver.OnReceiveAllInAuction(OwnerClientId);
        }

        public void SendBetAuction(int bet)
        {
            Debug.Log($"Player {OwnerClientId}: send bet '{bet}' auction to Master");
            InvokeServerRpc(MasterReceiveBetAuction, bet);
        }
        
        [ServerRPC]
        private void MasterReceiveBetAuction(int bet)
        {
            Debug.Log($"Master: Receive bet '{bet}' auction from Player {GetPlayerInfo()}");
            MasterDataReceiver.OnReceiveBetAuction(OwnerClientId, bet);
        }
        
        #endregion
    }
}