using Injection;

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
        
        private void OnPlayerDisconnect(NetworkPlayer networkPlayer)
        {
            Data.NetworkPlayers.Remove(networkPlayer);
        }
    }
}