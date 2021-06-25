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
        [Inject] private FinalRoundData FinalRoundData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PackagePlayStateData PackagePlayStateData { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public void OnReceiveRegisteredPlayerId(byte playerId)
        {
            NetworkData.RegisteredPlayerId = playerId;
            InitializePlayerData();
        }
        
        private void InitializePlayerData()
        {
            if (PlayersBoard.Players.Any(_ => _.PlayerId == NetworkData.RegisteredPlayerId))
            {
                MatchData.IsMeCurrentPlayer = PlayersBoardSystem.IsCurrentPlayer(NetworkData.RegisteredPlayerId);
                MatchData.ThisPlayer = PlayersBoardSystem.GetPlayer(NetworkData.RegisteredPlayerId);
            }
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

        public void OnReceiveNetRound(NetRound netRound)
        {
            //todo: Finish refactoring
            foreach (NetRoundQuestion roundQuestion in netRound.Themes.SelectMany(theme => theme.Questions))
                roundQuestion.IsDownloadedByMe = roundQuestion.FileIds.All(PlayerFilesRepository.IsDownloaded);

            MatchData.RoundData.Value = netRound;
        }
        
        public void OnReceiveFinalRoundData(FinalRoundData finalRoundData)
        {
            FinalRoundData.Update(finalRoundData);
            MetagameEvents.FinalRoundDataChanged.Publish();
        }

        public void OnReceivePackagePlayStateData(PackagePlayStateData data)
        {
            PackagePlayStateData.Update(data);
            MetagameEvents.PackagePlayStateChanged.Publish();
        }
        
        public void OnReceiveCommand(INetworkCommand command)
        {
            Debug.Log($"Receive command from Master, {command}");
            CommandsSystem.AddReceivedMasterCommand(command);
        }
    }
}