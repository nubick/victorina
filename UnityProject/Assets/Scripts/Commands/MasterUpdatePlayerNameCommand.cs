using Injection;
using UnityEngine;
using Victorina;
using Victorina.Commands;

namespace Commands
{
    public class MasterUpdatePlayerNameCommand : Command, IServerCommand
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        
        public override CommandType Type => CommandType.MasterUpdatePlayerName;
        
        public PlayerData Player { get; set; }
        public string NewPlayerName { get; set; }
        
        public bool CanExecuteOnServer()
        {
            if (!PlayersBoardSystem.IsPlayerNameValid(NewPlayerName))
            {
                Debug.Log($"Cmd: Can't update player name. New player name '{NewPlayerName}' is not valid.");
                return false;
            }
            
            return true;
        }

        public void ExecuteOnServer()
        {
            Debug.Log($"Update player name from '{Player.Name}' to '{NewPlayerName}'");
            ConnectionMessage msg = ConnectedPlayersData.GetByPlayerId(Player.PlayerId).ConnectionMessage;
            msg.Name = NewPlayerName;
            PlayersBoardSystem.UpdatePlayersBoard();
        }

        public override string ToString() => $"[MasterUpdatePlayerNameCommand, Player: {Player}, NewPlayerName: {NewPlayerName}]";
    }
}