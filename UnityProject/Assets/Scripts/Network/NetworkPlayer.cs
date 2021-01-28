using System;
using Injection;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Victorina
{
    public class NetworkPlayer : NetworkedBehaviour
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }
        [Inject] private ClientFilesRepository ClientFilesRepository { get; set; }
        
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
            netQuestion.QuestionStory = new StoryDot[netQuestion.StoryDotsAmount];
            MatchData.SelectedQuestion = netQuestion;
        }

        public void SendStoryDot(StoryDot storyDot)
        {
            Debug.Log($"Master: Send story dot: {storyDot} to {OwnerClientId}");
            if(storyDot is TextStoryDot textStoryDot)
                InvokeClientRpcOnOwner(ReceiveTextStoryDot, textStoryDot);
            else if (storyDot is ImageStoryDot imageStoryDot)
                InvokeClientRpcOnOwner(ReceiveImageStoryDot, imageStoryDot);
            else if(storyDot is AudioStoryDot audioStoryDot)
                InvokeClientRpcOnOwner(ReceiveAudioStoryDot, audioStoryDot);
            else if (storyDot is VideoStoryDot videoStoryDot)
                InvokeClientRpcOnOwner(ReceiveVideoStoryDot, videoStoryDot);
            else
                throw new Exception($"Not supported story dot: {storyDot}");
        }
        
        [ClientRPC]
        private void ReceiveTextStoryDot(TextStoryDot storyDot)
        {
            Debug.Log($"Player {OwnerClientId}: Receive text story dot: {storyDot}");
            MatchData.SelectedQuestion.QuestionStory[storyDot.Index] = storyDot;
        }

        [ClientRPC]
        private void ReceiveImageStoryDot(ImageStoryDot imageStoryDot)
        {
            Debug.Log($"Player {OwnerClientId}: Receive image story dot: {imageStoryDot}");
            MatchData.SelectedQuestion.QuestionStory[imageStoryDot.Index] = imageStoryDot;
            ClientFilesRepository.Register(imageStoryDot.FileId, imageStoryDot.ChunksAmount);
        }
        
        [ClientRPC]
        private void ReceiveAudioStoryDot(AudioStoryDot audioStoryDot)
        {
            Debug.Log($"Player {OwnerClientId}: Receive audio story dot: {audioStoryDot}");
            MatchData.SelectedQuestion.QuestionStory[audioStoryDot.Index] = audioStoryDot;
            ClientFilesRepository.Register(audioStoryDot.FileId, audioStoryDot.ChunksAmount);
        }
        
        [ClientRPC]
        private void ReceiveVideoStoryDot(VideoStoryDot videoStoryDot)
        {
            Debug.Log($"Player {OwnerClientId}: Receive video story dot: {videoStoryDot}");
            MatchData.SelectedQuestion.QuestionStory[videoStoryDot.Index] = videoStoryDot;
            ClientFilesRepository.Register(videoStoryDot.FileId, videoStoryDot.ChunksAmount);
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
                Debug.Log($"Master: Send file chunk [{fileId};{chunkIndex}]");
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
            Debug.Log($"Player {OwnerClientId}: Receive file chunk [{fileId};{chunkIndex}], bytes: {bytes.SizeKb()}");
            ClientFilesRepository.AddChunk(fileId, chunkIndex, bytes);
        }
        
        //==== To Master ====
        public void SendFileChunkRequest(int fileId, int chunkIndex)
        {
            Debug.Log($"Player {OwnerClientId}: Request file chunk [{fileId};{chunkIndex}]");
            InvokeServerRpc(ReceiveClientFileRequest, fileId, chunkIndex);
        }
        
        [ServerRPC]
        private void ReceiveClientFileRequest(int fileId, int chunkIndex)
        {
            Debug.Log($"Master: Receive Player {OwnerClientId} file chunk request: [{fileId};{chunkIndex}]");
            SendFileChunk(fileId, chunkIndex);
        }
        
        #endregion
    }
}