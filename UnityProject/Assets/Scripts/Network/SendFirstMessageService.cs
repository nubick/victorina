using System.Collections;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class SendFirstMessageService
    {
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }

        public IEnumerator SendCoroutine()
        {
            for (;;)
            {
                if (NetworkData.IsMaster)
                {
                    while (ConnectedPlayersData.WaitingFirstMessageClientIds.Count > 0)
                    {
                        ulong clientId = ConnectedPlayersData.WaitingFirstMessageClientIds.Dequeue();
                        SendFirstMessage(clientId);
                    }
                }
                yield return null;
            }
        }

        private void SendFirstMessage(ulong clientId)
        {
            Debug.Log($"Master: Send first message to clientId: {clientId}");
            NetworkPlayer networkPlayer = ServerService.GetNetworkPlayer(clientId);
            JoinedPlayer joinedPlayer = ConnectedPlayersData.GetByClientId(clientId);
            if (joinedPlayer == null)
            {
                PlayerRejectReason rejectReason = ConnectedPlayersData.RejectedPlayers[clientId];
                SendToPlayersService.SendRejectReason(networkPlayer, rejectReason);
            }
            else
            {
                SendToPlayersService.SendConnectionData(networkPlayer, joinedPlayer.PlayerId);
            }
        }
    }
}