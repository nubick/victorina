using System;
using Injection;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Victorina
{
    public class NetworkPlayer : NetworkedBehaviour
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }
        [Inject] private PlayerFilesRepository PlayerFilesRepository { get; set; }
        [Inject] private PlayerDataReceiver PlayerDataReceiver { get; set; }
        
        public void Awake()
        {
            Debug.Log("Network player created");
        }
        
        public void SendPlayersBoard(PlayersBoard playersBoard)
        {
            Debug.Log($"Master: Send PlayersBoard: {playersBoard} to {OwnerClientId}");
            InvokeClientRpcOnOwner(UpdatePlayersBoardRPC, playersBoard);
        }
        
        [ClientRPC]
        private void UpdatePlayersBoardRPC(PlayersBoard playersBoard)
        {
            Debug.Log($"Player {OwnerClientId}: Receive PlayersBoard: {playersBoard}");
            MatchData.PlayersBoard.Value = playersBoard;
        }

        public void SendMatchPhase(MatchPhase matchPhase)
        {
            Debug.Log($"Master: Send match phase: {matchPhase} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveMatchPhase, matchPhase);
        }

        [ClientRPC]
        private void ReceiveMatchPhase(MatchPhase matchPhase)
        {
            Debug.Log($"Player {OwnerClientId}: Receive match phase: {matchPhase}");
            MatchData.Phase.Value = matchPhase;
        }
        
        public void SendRoundData(NetRound netRound)
        {
            Debug.Log($"Master: Send RoundData: {netRound} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveRoundData, netRound, "RFS");
        }

        [ClientRPC]
        private void ReceiveRoundData(NetRound netRound)
        {
            Debug.Log($"Player {OwnerClientId}: Receive RoundData: {netRound}");
            MatchData.RoundData.Value = netRound;
        }
        
        public void SendSelectedQuestion(NetQuestion netQuestion)
        {
            Debug.Log($"Master: Send selected question: {netQuestion} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveSelectedQuestion, netQuestion, "RFS");
        }

        [ClientRPC]
        private void ReceiveSelectedQuestion(NetQuestion netQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive selected question: {netQuestion}");
            netQuestion.QuestionStory = new StoryDot[netQuestion.QuestionStoryDotsAmount];
            netQuestion.AnswerStory = new StoryDot[netQuestion.AnswerStoryDotsAmount];
            MatchData.SelectedQuestion.Value = netQuestion;
        }

        public void SendStoryDot(StoryDot storyDot, bool isQuestion)
        {
            Debug.Log($"Master: Send story dot: {storyDot} to {OwnerClientId}");
            if(storyDot is TextStoryDot textStoryDot)
                InvokeClientRpcOnOwner(ReceiveTextStoryDot, textStoryDot, isQuestion);
            else if (storyDot is ImageStoryDot imageStoryDot)
                InvokeClientRpcOnOwner(ReceiveImageStoryDot, imageStoryDot, isQuestion);
            else if(storyDot is AudioStoryDot audioStoryDot)
                InvokeClientRpcOnOwner(ReceiveAudioStoryDot, audioStoryDot, isQuestion);
            else if (storyDot is VideoStoryDot videoStoryDot)
                InvokeClientRpcOnOwner(ReceiveVideoStoryDot, videoStoryDot, isQuestion);
            else
                throw new Exception($"Not supported story dot: {storyDot}");
        }

        private void SetStoryDot(StoryDot storyDot, bool isQuestion)
        {
            if (isQuestion)
                MatchData.SelectedQuestion.Value.QuestionStory[storyDot.Index] = storyDot;
            else
                MatchData.SelectedQuestion.Value.AnswerStory[storyDot.Index] = storyDot;
        }
        
        [ClientRPC]
        private void ReceiveTextStoryDot(TextStoryDot textStoryDot, bool isQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive text story dot: {textStoryDot}");
            SetStoryDot(textStoryDot, isQuestion);
        }

        [ClientRPC]
        private void ReceiveImageStoryDot(ImageStoryDot imageStoryDot, bool isQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive image story dot: {imageStoryDot}");
            SetStoryDot(imageStoryDot, isQuestion);
            PlayerFilesRepository.Register(imageStoryDot.FileId, imageStoryDot.ChunksAmount);
        }
        
        [ClientRPC]
        private void ReceiveAudioStoryDot(AudioStoryDot audioStoryDot, bool isQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive audio story dot: {audioStoryDot}");
            SetStoryDot(audioStoryDot, isQuestion);
            PlayerFilesRepository.Register(audioStoryDot.FileId, audioStoryDot.ChunksAmount);
        }

        [ClientRPC]
        private void ReceiveVideoStoryDot(VideoStoryDot videoStoryDot, bool isQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive video story dot: {videoStoryDot}");
            SetStoryDot(videoStoryDot, isQuestion);
            PlayerFilesRepository.Register(videoStoryDot.FileId, videoStoryDot.ChunksAmount);
        }

        public void SendSelectedRoundQuestion(NetRoundQuestion netRoundQuestion)
        {
            Debug.Log($"Master: Send selected round question: {netRoundQuestion} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveSelectedRoundQuestion, netRoundQuestion);
        }

        [ClientRPC]
        private void ReceiveSelectedRoundQuestion(NetRoundQuestion netRoundQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive selected round question: {netRoundQuestion}");
            MatchData.SelectedRoundQuestion = netRoundQuestion;
        }

        public void SendCurrentStoryDotIndex(int index)
        {
            Debug.Log($"Master: Send current story dot index: {index} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveCurrentStoryDotIndex, index);
        }

        [ClientRPC]
        private void ReceiveCurrentStoryDotIndex(int index)
        {
            Debug.Log($"Player {OwnerClientId}: Receive current story dot index: {index}");
            MatchData.CurrentStoryDotIndex.Value = index;
        }

        public void SendNetRoundsInfo(NetRoundsInfo netRoundsInfo)
        {
            Debug.Log($"Master: Send rounds info: {netRoundsInfo} to {OwnerClientId}");
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
                Debug.Log($"Master: Send file chunk [{fileId};{chunkIndex}] to Player {OwnerClientId}");
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
            Debug.Log($"Player {OwnerClientId}: Receive file chunk [{fileId};{chunkIndex}], bytes: {bytes.SizeKb()}, ({Time.time:0.000})");
            PlayerFilesRepository.AddChunk(fileId, chunkIndex, bytes);
        }
        
        public void SendFileChunkRequestToMaster(int fileId, int chunkIndex)
        {
            Debug.Log($"Player {OwnerClientId}: Request file chunk [{fileId};{chunkIndex}], ({Time.time:0.000})");
            InvokeServerRpc(MasterReceiveClientFileRequest, fileId, chunkIndex);
        }
        
        [ServerRPC]
        private void MasterReceiveClientFileRequest(int fileId, int chunkIndex)
        {
            Debug.Log($"Master: Receive Player {OwnerClientId} file chunk request: [{fileId};{chunkIndex}]");
            SendFileChunk(fileId, chunkIndex);
        }

        public void SendRoundFileIds(int[] fileIds, int[] chunksAmounts)
        {
            Debug.Log($"Master: Send round of file ids ({fileIds.Length}) to {IsOwner}");
            InvokeClientRpcOnOwner(ReceiveRoundFileIds, fileIds, chunksAmounts);
        }

        [ClientRPC]
        private void ReceiveRoundFileIds(int[] fileIds, int[] chunksAmounts)
        {
            Debug.Log($"Player {OwnerClientId}: Receive round file ids, amount: {fileIds.Length}");
            PlayerFilesRepository.Register(fileIds, chunksAmounts);
        }
        
        #endregion

        #region Timer and button

        public void SendStartTimer(float resetSeconds, float leftSeconds)
        {
            Debug.Log($"Master: Send start timer to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveStartTimer, resetSeconds, leftSeconds);
        }

        [ClientRPC]
        private void ReceiveStartTimer(float resetSeconds, float leftSeconds)
        {
            Debug.Log($"Player {OwnerClientId}: Receive start timer");
            PlayerDataReceiver.OnReceiveStartTimer(resetSeconds, leftSeconds);
        }

        public void SendStopTimer()
        {
            Debug.Log($"Master: Send stop timer to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveStopTimer);
        }

        [ClientRPC]
        private void ReceiveStopTimer()
        {
            Debug.Log($"Player {OwnerClientId}: Receive stop timer");
            PlayerDataReceiver.OnReceiveStopTimer();
        }

        public void SendPlayerButtonClickToMaster(float thoughtSeconds)
        {
            Debug.Log($"Player {OwnerClientId}: send button click to Master, thoughtSeconds: {thoughtSeconds}");
            InvokeServerRpc(MasterReceivePlayerButtonClick, thoughtSeconds);
        }
        
        [ServerRPC]
        private void MasterReceivePlayerButtonClick(float thoughtSeconds)
        {
            Debug.Log($"Master: Receive Player {OwnerClientId} button click, thoughtSeconds: {thoughtSeconds}");
            MatchSystem.OnPlayerButtonClickReceived(OwnerClientId, thoughtSeconds);
        }

        public void SendPlayersButtonClickData(PlayersButtonClickData playerButtonClickData)
        {
            Debug.Log($"Master: Send players button click data to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceivePlayersButtonClickData, playerButtonClickData);
        }

        [ClientRPC]
        private void ReceivePlayersButtonClickData(PlayersButtonClickData playersButtonClickData)
        {
            Debug.Log($"Player {OwnerClientId}: Receive players button click data");
            PlayerDataReceiver.OnReceive(playersButtonClickData);
        }
        
        #endregion
    }
}