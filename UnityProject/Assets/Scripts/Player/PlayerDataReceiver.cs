using System.Linq;
using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class PlayerDataReceiver
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private PlayerFilesRepository PlayerFilesRepository { get; set; }
        [Inject] private PlayerFilesRequestSystem PlayerFilesRequestSystem { get; set; }
        [Inject] private PlayEffectsSystem PlayEffectsSystem { get; set; }
        [Inject] private AcceptingAnswerTimerData AcceptingAnswerTimerData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private ServerEventsSystem ServerEventsSystem { get; set; }
        
        public void OnReceiveRegisteredPlayerId(byte playerId)
        {
            NetworkData.RegisteredPlayerId = playerId;
            InitializePlayerData();
            NetworkData.ClientConnectingState = ClientConnectingState.Success;
            MetagameEvents.ConnectedAsClient.Publish();
        }
        
        private void InitializePlayerData()
        {
            if (PlayersBoard.Players.Any(_ => _.PlayerId == NetworkData.RegisteredPlayerId))
            {
                MatchData.IsMeCurrentPlayer = PlayersBoardSystem.IsCurrentPlayer(NetworkData.RegisteredPlayerId);
                MatchData.ThisPlayer = PlayersBoardSystem.GetPlayer(NetworkData.RegisteredPlayerId);
            }
        }

        public void OnReceiveRejectReason(PlayerRejectReason rejectReason)
        {
            Debug.Log($"Reject Reason: {rejectReason}");
            NetworkData.PlayerRejectReason = rejectReason;
            NetworkData.ClientConnectingState = ClientConnectingState.Rejected;
        }
        
        public void OnReceive(PlayersBoard playersBoard)
        {
            PlayersBoard.Update(playersBoard);
            MetagameEvents.PlayersBoardChanged.Publish();
            if (NetworkData.RegisteredPlayerId > 0)
                InitializePlayerData();
        }
        
        public void OnReceive(PlayersButtonClickData data)
        {
            PlayersButtonClickData.Update(data);
            MetagameEvents.PlayersButtonClickDataChanged.Publish();
        }
        
        public void OnReceiveAnswerTimerData(AnswerTimerData data)
        {
            AnswerTimerData.Update(data);
            MetagameEvents.AnswerTimerDataChanged.Publish();
        }
        
        public void OnRoundFileIdsReceived(int[] fileIds, int[] chunksAmounts, int[] priorities)
        {
            PlayerFilesRepository.Register(fileIds, chunksAmounts, priorities);
            PlayerFilesRequestSystem.SendLoadingProgress();
        }
        
        public void OnFileChunkReceived(int fileId, int chunkIndex, byte[] bytes)
        {
            PlayerFilesRepository.AddChunk(fileId, chunkIndex, bytes);
        }

        public void OnReceivePlaySoundEffectCommand(int number)
        {
            PlayEffectsSystem.PlaySound(number);
        }

        public void OnReceiveAcceptingAnswerTimerData(bool isRunning, float maxSeconds, float leftSeconds)
        {
            AcceptingAnswerTimerData.IsRunning = isRunning;
            AcceptingAnswerTimerData.MaxSeconds = maxSeconds;
            AcceptingAnswerTimerData.LeftSeconds = leftSeconds;
        }
        
        public void OnReceivePackagePlayStateData(PlayStateData data)
        {
            PlayStateData.Update(data);
            MetagameEvents.PlayStateChanged.Publish();
        }
        
        public void OnReceiveCommand(INetworkCommand command)
        {
            Debug.Log($"Receive command from Master, {command}");
            CommandsSystem.AddReceivedMasterCommand(command);
        }
        
        public void OnReceiveServerEvent(string serverEventId, ServerEventArgument argument)
        {
            Debug.Log($"Receive server event: {serverEventId}, {argument}");
            ServerEventsSystem.OnPlayerReceiveServerEvent(serverEventId, argument);
        }
    }
}