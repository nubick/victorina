using System.Linq;
using Assets.Scripts.Utils;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayersBoardSystem
    {
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private ServerService ServerService { get; set; }

        private PlayersBoard PlayersBoard => MatchData.PlayersBoard.Value;
        
        public void Initialize()
        {
            MetagameEvents.MasterClientConnected.Subscribe(UpdatePlayersBoard);
            MetagameEvents.MasterClientDisconnected.Subscribe(UpdatePlayersBoard);
            MetagameEvents.ServerStopped.Subscribe(OnServerStopped);
        }
        
        private void UpdatePlayersBoard()
        {
            if (NetworkData.IsClient)
                return;
            
            PlayersBoard.Players.ForEach(_ => _.IsConnected = false);
            foreach (ulong connectedClientId in ConnectedPlayersData.ConnectedClientsIds)
            {
                ConnectionMessage msg = ConnectedPlayersData.PlayersIdToConnectionMessageMap[connectedClientId];
                PlayerData player = PlayersBoard.Players.SingleOrDefault(_ => _.ServerGuid == msg.Guid);
                
                if (player == null)
                {
                    player = new PlayerData();
                    player.ServerGuid = msg.Guid;
                    PlayersBoard.Players.Add(player);
                }
                
                player.Id = connectedClientId;
                player.Name = msg.Name;
                player.IsConnected = true;
            }

            PlayersBoard.Players.Where(_ => !_.IsConnected).ForEach(_ => _.Id = 0);

            if (PlayersBoard.Current != null && !PlayersBoard.Current.IsConnected)
                PlayersBoard.Current = null;
            
            MatchData.PlayersBoard.NotifyChanged();
            SendToPlayersService.Send(PlayersBoard);
        }

        private void OnServerStopped()
        {
            if (NetworkData.IsClient)
                return;

            PlayersBoard.Players.Clear();
        }

        public void MakePlayerCurrent(ulong playerId)
        {
            if (NetworkData.IsClient)
                return;

            PlayerData playerData = MatchSystem.GetPlayer(playerId);
            MakePlayerCurrent(playerData);
        }
        
        public void MakePlayerCurrent(PlayerData playerData)
        {
            if (NetworkData.IsClient)
                return;

            if (PlayersBoard.Current == playerData)
                return;
            
            Debug.Log($"MakePlayerCurrent: {playerData}");
            PlayersBoard.Current = playerData;
            MatchData.PlayersBoard.NotifyChanged();
            SendToPlayersService.Send(MatchData.PlayersBoard.Value);
        }

        public void UpdateFilesLoadingPercentage(ulong playerId, byte percentage)
        {
            if (NetworkData.IsClient)
                return;

            PlayerData playerData = PlayersBoard.Players.SingleOrDefault(_ => _.Id == playerId);
            if (playerData == null)
            {
                Debug.Log($"Master. Validation Error. Can't find player by id '{playerId}' to set files loading percentage: {percentage}");
            }
            else
            {
                playerData.FilesLoadingPercentage = percentage;
                MatchData.PlayersBoard.NotifyChanged();
                SendToPlayersService.Send(PlayersBoard);
            }
        }

        public void UpdatePlayerName(PlayerData playerData, string newPlayerName)
        {
            Debug.Log($"Update player name from '{playerData.Name}' to '{newPlayerName}'");
            if (ServerService.IsPlayerNameValid(newPlayerName))
            {
                ConnectionMessage msg = ConnectedPlayersData.PlayersIdToConnectionMessageMap.Values.Single(_ => _.Guid == playerData.ServerGuid);
                msg.Name = newPlayerName;
                UpdatePlayersBoard();
            }
            else
            {
                Debug.LogError($"New player name '{newPlayerName}' is not valid/");
            }
        }

        public void UpdatePlayerScore(PlayerData playerData, int newScore)
        {
            Debug.Log($"Update player score from '{playerData.Score}' to '{newScore}'");
            playerData.Score = newScore;
            UpdatePlayersBoard();
        }
    }
}