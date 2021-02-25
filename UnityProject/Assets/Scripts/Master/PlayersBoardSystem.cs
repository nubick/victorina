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

        private PlayersBoard PlayersBoard => MatchData.PlayersBoard.Value;
        
        public void Initialize()
        {
            MetagameEvents.PlayerConnected.Subscribe(_ => UpdatePlayersBoard());
            MetagameEvents.PlayerDisconnected.Subscribe(UpdatePlayersBoard);
            MetagameEvents.ServerStopped.Subscribe(OnServerStopped);
        }
        
        private void UpdatePlayersBoard()
        {
            foreach (ulong connectedClientId in ConnectedPlayersData.ConnectedClientsIds)
            {
                PlayerData player = PlayersBoard.Players.SingleOrDefault(_ => _.Id == connectedClientId);
                if (player == null)
                {
                    player = new PlayerData(connectedClientId);
                    player.Name = ConnectedPlayersData.PlayersIdToNameMap[connectedClientId];
                    PlayersBoard.Players.Add(player);
                }
                
            }
            MatchData.PlayersBoard.NotifyChanged();
            SendToPlayersService.Send(PlayersBoard);
        }

        private void OnServerStopped()
        {
            PlayersBoard.Players.Clear();
        }

        public void MakePlayerCurrent(ulong playerId)
        {
            PlayerData playerData = MatchSystem.GetPlayer(playerId);
            MakePlayerCurrent(playerData);
        }
        
        public void MakePlayerCurrent(PlayerData playerData)
        {
            if (PlayersBoard.Current == playerData)
                return;
            
            Debug.Log($"MakePlayerCurrent: {playerData}");
            PlayersBoard.Current = playerData;
            MatchData.PlayersBoard.NotifyChanged();
            SendToPlayersService.Send(MatchData.PlayersBoard.Value);
        }

        public void UpdateFilesLoadingPercentage(ulong playerId, byte percentage)
        {
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
    }
}