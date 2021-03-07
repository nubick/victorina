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
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }
        [Inject] private PlayerDataReceiver PlayerDataReceiver { get; set; }
        [Inject] private MasterDataReceiver MasterDataReceiver { get; set; }

        public void Awake()
        {
            Debug.Log("Network player created");
        }

        public void SendPlayersBoard(PlayersBoard playersBoard)
        {
            //Debug.Log($"Master: Send PlayersBoard: {playersBoard} to {OwnerClientId}");
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
            Debug.Log($"Player {OwnerClientId}: Receive RoundData: {netRound}");
            MatchData.RoundData.Value = netRound;
        }

        public void SendQuestionAnswerData(QuestionAnswerData questionAnswerData)
        {
            //Debug.Log($"Master: Send question answer data: {questionAnswerData} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveQuestionAnswerData, questionAnswerData);
        }

        [ClientRPC]
        private void ReceiveQuestionAnswerData(QuestionAnswerData questionAnswerData)
        {
            Debug.Log($"Player {OwnerClientId}: Receive question answer data: {questionAnswerData}, {Time.time}");
            PlayerDataReceiver.OnReceive(questionAnswerData);
        }

        public void SendSelectedQuestion(NetQuestion netQuestion)
        {
            //Debug.Log($"Master: Send selected question: {netQuestion} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveSelectedQuestion, netQuestion);

            foreach (StoryDot storyDot in netQuestion.QuestionStory)
            {
                //Debug.Log($"Master: Send question story dot to All: {storyDot}");
                SendStoryDot(storyDot, isQuestion: true);
            }

            foreach (StoryDot storyDot in netQuestion.AnswerStory)
            {
                //Debug.Log($"Master: Send answer story dot to All: {storyDot}");
                SendStoryDot(storyDot, isQuestion: false);
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

        public void SendStoryDot(StoryDot storyDot, bool isQuestion)
        {
            //Debug.Log($"Master: Send story dot: {storyDot} to {OwnerClientId}");
            if (storyDot is TextStoryDot textStoryDot)
                InvokeClientRpcOnOwner(ReceiveTextStoryDot, textStoryDot, isQuestion);
            else if (storyDot is ImageStoryDot imageStoryDot)
                InvokeClientRpcOnOwner(ReceiveImageStoryDot, imageStoryDot, isQuestion);
            else if (storyDot is AudioStoryDot audioStoryDot)
                InvokeClientRpcOnOwner(ReceiveAudioStoryDot, audioStoryDot, isQuestion);
            else if (storyDot is VideoStoryDot videoStoryDot)
                InvokeClientRpcOnOwner(ReceiveVideoStoryDot, videoStoryDot, isQuestion);
            else if (storyDot is NoRiskStoryDot noRiskStoryDot)
                InvokeClientRpcOnOwner(ReceiveNoRiskStoryDot, noRiskStoryDot);
            else
                throw new Exception($"Not supported story dot: {storyDot}");
        }

        private void SetStoryDot(StoryDot storyDot, bool isQuestion)
        {
            if (isQuestion)
                MatchData.QuestionAnswerData.SelectedQuestion.Value.QuestionStory[storyDot.Index] = storyDot;
            else
                MatchData.QuestionAnswerData.SelectedQuestion.Value.AnswerStory[storyDot.Index] = storyDot;
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
            PlayerDataReceiver.OnFileStoryDotReceived(imageStoryDot);
        }

        [ClientRPC]
        private void ReceiveAudioStoryDot(AudioStoryDot audioStoryDot, bool isQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive audio story dot: {audioStoryDot}");
            SetStoryDot(audioStoryDot, isQuestion);
            PlayerDataReceiver.OnFileStoryDotReceived(audioStoryDot);
        }

        [ClientRPC]
        private void ReceiveVideoStoryDot(VideoStoryDot videoStoryDot, bool isQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive video story dot: {videoStoryDot}");
            SetStoryDot(videoStoryDot, isQuestion);
            PlayerDataReceiver.OnFileStoryDotReceived(videoStoryDot);
        }

        [ClientRPC]
        private void ReceiveNoRiskStoryDot(NoRiskStoryDot noRiskStoryDot)
        {
            Debug.Log($"Player {OwnerClientId}: Receive no risk story dot");
            SetStoryDot(noRiskStoryDot, true);
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

        public void SendRoundFileIds(int[] fileIds, int[] chunksAmounts)
        {
            //Debug.Log($"Master: Send round of file ids ({fileIds.Length}) to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveRoundFileIds, fileIds, chunksAmounts);
        }

        [ClientRPC]
        private void ReceiveRoundFileIds(int[] fileIds, int[] chunksAmounts)
        {
            Debug.Log($"Player {OwnerClientId}: Receive round file ids, amount: {fileIds.Length}");
            PlayerDataReceiver.OnRoundFileIdsReceived(fileIds, chunksAmounts);
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
            Debug.Log($"Master: Receive Player {OwnerClientId} button click, thoughtSeconds: {spentSeconds}");
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
            Debug.Log($"Player {OwnerClientId}: Receive players button click data: {playersButtonClickData}, {Time.time}");
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
            Debug.Log($"Master: Receive select round question, Player {OwnerClientId}, netRoundQuestion: {netRoundQuestion}");
            MasterDataReceiver.OnCurrentPlayerSelectRoundQuestionReceived(OwnerClientId, netRoundQuestion);
        }
        
        #endregion

        #region Files Loading Percentage to MASTER

        public void SendFilesLoadingPercentageToMaster(byte percentage)
        {
            Debug.Log($"Player {OwnerClientId}: send files loading percentage to Master: {percentage}");
            InvokeServerRpc(MasterReceiveFilesLoadingPercentage, percentage);
        }
        
        [ServerRPC]
        private void MasterReceiveFilesLoadingPercentage(byte percentage)
        {
            Debug.Log($"Master: Receive files loading percentage '{percentage}' from Player {OwnerClientId}");
            MasterDataReceiver.OnFilesLoadingPercentageReceived(OwnerClientId, percentage);
        }
        
        #endregion
    }
}