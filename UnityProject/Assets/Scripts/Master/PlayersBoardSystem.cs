using System;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayersBoardSystem
    {
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }

        public void Initialize()
        {
            MetagameEvents.MasterClientConnected.Subscribe(_ => UpdatePlayersBoard());
            MetagameEvents.MasterClientDisconnected.Subscribe(UpdatePlayersBoard);
            MetagameEvents.ServerStopped.Subscribe(OnServerStopped);
        }
        
        public void UpdatePlayersBoard()
        {
            if (NetworkData.IsClient)
                return;
            
            PlayersBoard.Players.ForEach(_ => _.IsConnected = false);

            foreach (JoinedPlayer joinedPlayer in ConnectedPlayersData.Players)
            {
                PlayerData boardPlayer = PlayersBoard.Players.SingleOrDefault(_ => _.PlayerId == joinedPlayer.PlayerId);
                if (boardPlayer == null)
                {
                    boardPlayer = new PlayerData(joinedPlayer.PlayerId);
                    PlayersBoard.Players.Add(boardPlayer);
                }
                
                boardPlayer.Name = joinedPlayer.ConnectionMessage.Name;
                boardPlayer.IsConnected = joinedPlayer.IsConnected;
            }
        }

        private void OnServerStopped()
        {
            if (NetworkData.IsMaster)
            {
                PlayersBoard.Clear();
            }
        }

        public void MakePlayerCurrent(byte playerId)
        {
            if (NetworkData.IsMaster)
            {
                PlayerData playerData = GetPlayer(playerId);
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
            PlayersBoard.SetCurrent(playerData);
        }

        public void UpdateFilesLoadingPercentage(byte playerId, byte percentage)
        {
            if (NetworkData.IsClient)
                return;

            PlayerData playerData = GetPlayer(playerId);
            playerData.FilesLoadingPercentage = percentage;
            PlayersBoard.MarkAsChanged();
        }
        
        public bool IsPlayerNameValid(string playerName)
        {
            return !string.IsNullOrWhiteSpace(playerName);
        }
        
        public PlayerData GetPlayer(byte playerId)
        {
            PlayerData player = PlayersBoard.Players.SingleOrDefault(_ => _.PlayerId == playerId);
            if (player == null)
                throw new Exception($"Can't find player with id: {playerId}");
            return player;
        }
        
        public bool IsCurrentPlayer(byte playerId)
        {
            return PlayersBoard.Current != null && PlayersBoard.Current.PlayerId == playerId;
        }

        public bool IsCurrentPlayer(PlayerData player)
        {
            return IsCurrentPlayer(player.PlayerId);
        }
        
        public string GetCurrentPlayerName()
        {
            return PlayersBoard.Current == null ? Static.EmptyPlayerName : PlayersBoard.Current.Name;
        }

        public string GetPlayerName(byte playerId)
        {
            return GetPlayer(playerId).Name;
        }
        
        public void RewardPlayer(byte playerId, int price)
        {
            PlayerData player = GetPlayer(playerId);
            RewardPlayer(player, price);
        }

        public void RewardPlayer(PlayerData player, int price)
        {
            player.Score += price;
            Debug.Log($"Reward player {player} by {price}, new score: {player.Score}");
            PlayersBoard.MarkAsChanged();
        }
        
        public void FinePlayer(byte playerId, int price)
        {
            PlayerData player = GetPlayer(playerId);
            FinePlayer(player, price);
        }

        public void FinePlayer(PlayerData player, int price)
        {
            player.Score -= price;
            Debug.Log($"Fine player {player} by {price}, new score: {player.Score}");
            PlayersBoard.MarkAsChanged();
        }
    }
}