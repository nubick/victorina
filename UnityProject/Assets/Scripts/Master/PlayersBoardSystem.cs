using System.Linq;
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
            MetagameEvents.MasterClientConnected.Subscribe(_ => UpdatePlayersBoard());
            MetagameEvents.MasterClientDisconnected.Subscribe(UpdatePlayersBoard);
            MetagameEvents.ServerStopped.Subscribe(OnServerStopped);
        }
        
        private void UpdatePlayersBoard()
        {
            if (NetworkData.IsClient)
                return;
            
            PlayersBoard.Players.ForEach(_ => _.IsConnected = false);

            foreach (JoinedPlayer joinedPlayer in ConnectedPlayersData.Players)
            {
                PlayerData boardPlayer = PlayersBoard.Players.SingleOrDefault(_ => _.PlayerId == joinedPlayer.PlayerId);
                if (boardPlayer == null)
                {
                    boardPlayer = new PlayerData();
                    boardPlayer.PlayerId = joinedPlayer.PlayerId;
                    PlayersBoard.Players.Add(boardPlayer);
                }
                
                boardPlayer.Name = joinedPlayer.ConnectionMessage.Name;
                boardPlayer.IsConnected = true;
            }
            
            MatchData.PlayersBoard.NotifyChanged();
            SendToPlayersService.Send(PlayersBoard);
        }

        private void OnServerStopped()
        {
            if (NetworkData.IsMaster)
            {
                PlayersBoard.Players.Clear();
            }
        }

        public void MakePlayerCurrent(byte playerId)
        {
            if (NetworkData.IsMaster)
            {
                PlayerData playerData = MatchSystem.GetPlayer(playerId);
                MakePlayerCurrent(playerData);
            }
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

        public void UpdateFilesLoadingPercentage(byte playerId, byte percentage)
        {
            if (NetworkData.IsClient)
                return;

            PlayerData playerData = MatchSystem.GetPlayer(playerId);
            playerData.FilesLoadingPercentage = percentage;
            MatchData.PlayersBoard.NotifyChanged();
            SendToPlayersService.Send(PlayersBoard);
        }

        public void UpdatePlayerName(PlayerData playerData, string newPlayerName)
        {
            Debug.Log($"Update player name from '{playerData.Name}' to '{newPlayerName}'");
            if (ServerService.IsPlayerNameValid(newPlayerName))
            {
                ConnectionMessage msg = ConnectedPlayersData.Players.Single(_ => _.PlayerId == playerData.PlayerId).ConnectionMessage;
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