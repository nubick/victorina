using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class MasterDataReceiver
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private FilesDeliveryStatusManager FilesDeliveryStatusManager { get; set; }
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public void OnFilesLoadingPercentageReceived(ulong clientId, byte percentage, int[] downloadedFilesIds)
        {
            byte playerId = ConnectedPlayersData.GetPlayerId(clientId);
            PlayersBoardSystem.UpdateFilesLoadingPercentage(playerId, percentage);
            FilesDeliveryStatusManager.UpdateDownloadedFilesIds(playerId, downloadedFilesIds);
        }

        private PlayerData GetPlayer(ulong clientId)
        {
            byte playerId = ConnectedPlayersData.GetPlayerId(clientId);
            return PlayersBoardSystem.GetPlayer(playerId);
        }
        
        public void OnReceiveCommand(ulong clientId, INetworkCommand command)
        {
            PlayerData player = GetPlayer(clientId);
            Debug.Log($"Master: Receive command from Player {player}: '{command}'");
            CommandsSystem.AddReceivedPlayerCommand(command, player);
        }
    }
}