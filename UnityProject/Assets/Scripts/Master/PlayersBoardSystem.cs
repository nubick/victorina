using System.Linq;
using Injection;

namespace Victorina
{
    public class PlayersBoardSystem
    {
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
        public void Initialize()
        {
            MetagameEvents.PlayerConnected.Subscribe(_ => UpdatePlayersBoard());
            MetagameEvents.PlayerDisconnected.Subscribe(UpdatePlayersBoard);
            MetagameEvents.ServerStopped.Subscribe(OnServerStopped);
        }
        
        private void UpdatePlayersBoard()
        {
            PlayersBoard playersBoard = MatchData.PlayersBoard.Value;
            foreach (ulong connectedClientId in ConnectedPlayersData.ConnectedClientsIds)
            {
                PlayerData player = playersBoard.Players.SingleOrDefault(_ => _.Id == connectedClientId);
                if (player == null)
                {
                    player = new PlayerData(connectedClientId);
                    player.Name = ConnectedPlayersData.PlayersIdToNameMap[connectedClientId];
                    playersBoard.Players.Add(player);
                }
                
            }
            MatchData.PlayersBoard.NotifyChanged();
            SendToPlayersService.Send(playersBoard);
        }

        private void OnServerStopped()
        {
            MatchData.PlayersBoard.Value.Players.Clear();
        }
    }
}