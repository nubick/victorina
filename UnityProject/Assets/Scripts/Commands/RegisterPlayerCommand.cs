using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class RegisterPlayerCommand : Command, IServerCommand
    {
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        
        public string Guid { get; set; }
        public string Name { get; set; }
        
        public override CommandType Type => CommandType.RegisterPlayer;
        
        public bool CanExecuteOnServer()
        {
            JoinedPlayer player = ConnectedPlayersData.GetByGuid(Guid);
            if (player != null)
            {
                Debug.Log($"Can't register player. Player '{player.Name}' exists with guild: {Guid}");
                return false;
            }
            return true;
        }

        public void ExecuteOnServer()
        {
            JoinedPlayer player = new JoinedPlayer();
            player.Guid = Guid;
            player.Name = Name;
            player.PlayerId = ConnectedPlayersData.NextPlayerId;
            ConnectedPlayersData.NextPlayerId++;
            ConnectedPlayersData.Players.Add(player);
            PlayersBoardSystem.UpdatePlayersBoard();
            Debug.Log($"New player is registered, Name {player.Name}, Guid: {player.Guid}");
        }

        public override string ToString()
        {
            return $"[RegisterPlayerCommand, Name: {Name}, Guid: {Guid}]";
        }
    }
}