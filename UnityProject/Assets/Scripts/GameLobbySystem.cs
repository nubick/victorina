using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class GameLobbySystem
    {
        [Inject] private GameLobbyData Data { get; set; }
        [Inject] private GameLobbyView GameLobbyView { get; set; }
        
        public void Initialize()
        {
            MetagameEvents.PlayerConnected.Subscribe(OnPlayerConnected);
            MetagameEvents.PlayerDisconnect.Subscribe(OnPlayerDisconnect);
        }
        
        private void OnPlayerConnected(NetworkPlayer networkPlayer)
        {
            Data.NetworkPlayers.Add(networkPlayer);
            GameLobbyView.RefreshUI();
        }
        
        private void OnPlayerDisconnect(ulong clientId)
        {
            NetworkPlayer networkPlayer = Data.NetworkPlayers.SingleOrDefault(_ => _.NetworkedObject.NetworkId == clientId);

            if (networkPlayer == null)
            {
                Debug.Log($"Can't find NetworkPlayer with clientId: {clientId}");
            }
            else
            {
                Data.NetworkPlayers.Remove(networkPlayer);
                GameLobbyView.RefreshUI();
            }
        }
    }
}