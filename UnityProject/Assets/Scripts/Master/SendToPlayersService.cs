using System;
using System.Collections.Generic;
using System.Linq;
using Injection;
using MLAPI;
using MLAPI.Connection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class SendToPlayersService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        
        private string All => $"All: ({GetPlayersInfo()})";
        
        private List<NetworkPlayer> GetPlayers()
        {
            return NetworkingManager.ConnectedClientsList.Where(_ => _.PlayerObject != null).Select(_ => _.PlayerObject.GetComponent<NetworkPlayer>()).ToList();
        }

        private NetworkPlayer GetPlayer(PlayerData player)
        {
            ulong clientId = ConnectedPlayersData.GetClientId(player.PlayerId);
            NetworkedClient networkedClient = NetworkingManager.ConnectedClientsList.SingleOrDefault(_ => _.ClientId == clientId);
            if (networkedClient == null)
                throw new Exception($"Can't find networkedClient for player {player}");
            return networkedClient.PlayerObject.GetComponent<NetworkPlayer>();
        }

        public void SendConnectionData(NetworkPlayer networkPlayer, byte registeredPlayerId)
        {
            networkPlayer.SendRegisteredPlayerId(registeredPlayerId);
            
            Debug.Log($"Master: Send PlayersBoard: {PlayersBoard} to {networkPlayer.GetPlayerInfo()}");
            networkPlayer.SendPlayersBoard(PlayersBoard);

            Debug.Log($"Master: Send PlayStateData: {PlayStateData} to {networkPlayer.GetPlayerInfo()}");
            networkPlayer.SendPackagePlayStateData(PlayStateData);

            networkPlayer.SendPlayersButtonClickData(PlayersButtonClickData);

            (int[] fileIds, int[] chunksAmounts, int[] priorities) info = PackageSystem.GetPackageFilesInfo(PackageData.Package);
            networkPlayer.SendRoundFileIds(info.fileIds, info.chunksAmounts, info.priorities);
        }

        private string GetPlayersInfo()
        {
            return string.Join(",", GetPlayers().Select(_ => _.OwnerClientId.ToString()));
        }
        
        public void Send(PlayersBoard playersBoard)
        {
            //Debug.Log($"Master: Send PlayersBoard to {All}: {playersBoard}");
            GetPlayers().ForEach(player => player.SendPlayersBoard(playersBoard));
        }
        
        public void SendPlayersButtonClickData(PlayersButtonClickData playersButtonClickData)
        {
            Debug.Log($"Master: Send players button click data to {All}, {playersButtonClickData})");
            GetPlayers().ForEach(player => player.SendPlayersButtonClickData(playersButtonClickData));
        }

        public void SendAnswerTimerData(AnswerTimerData data)
        {
            Debug.Log($"Master: Send AnswerTimerData to {All}, {data})");
            GetPlayers().ForEach(player => player.SendAnswerTimerData(data));
        }
        
        public void SendPlaySoundEffectCommand(int number)
        {
            Debug.Log($"Master: Send play sound effect command to {All}, number: {number}");
            GetPlayers().ForEach(player => player.SendPlaySoundEffectCommand(number));
        }

        public void SendAcceptingAnswerTimerData(AcceptingAnswerTimerData data)
        {
            //Debug.Log($"Master: Send answering timer data to {All}, {data}");
            GetPlayers().ForEach(player => player.SendAcceptingAnswerTimerData(data));
        }
        
        public void SendFinalRoundData(FinalRoundData finalRoundData)
        {
            Debug.Log($"Master: Send FinalRoundData to {All}: {finalRoundData}");
            GetPlayers().ForEach(player => player.SendFinalRoundData(finalRoundData));
        }

        public void SendPackagePlayStateData(PlayStateData data)
        {
            Debug.Log($"Master: Send PlayStateData to {All}: {data}");
            GetPlayers().ForEach(player => player.SendPackagePlayStateData(data));
        }
        
        public void SendCommand(IndividualPlayerCommand command)
        {
            Debug.Log($"Master: Send individual player command to {command.Receiver}");
            NetworkPlayer networkPlayer = GetPlayer(command.Receiver);
            networkPlayer.SendCommandToPlayer(command);
        }

        public void SendCommand(IPlayerCommand playerCommand)
        {
            Debug.Log($"Master: Send PlayerCommand to {All}: {playerCommand}");
            GetPlayers().ForEach(player => player.SendCommandToPlayer(playerCommand));
        }
    }
}