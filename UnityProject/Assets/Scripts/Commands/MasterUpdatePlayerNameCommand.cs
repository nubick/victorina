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
        
        public byte PlayerId { get; }
        public string NewPlayerName { get; }

        public override CommandType Type => CommandType.MasterUpdatePlayerName;
        private PlayerData Player => PlayersBoardSystem.GetPlayer(PlayerId);
        
        public MasterUpdatePlayerNameCommand(byte playerId, string newPlayerName)
        {
            PlayerId = playerId;
            NewPlayerName = newPlayerName;
        }
        
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
            JoinedPlayer joinedPlayer = ConnectedPlayersData.GetByPlayerId(Player.PlayerId);
            joinedPlayer.Name = NewPlayerName;
            PlayersBoardSystem.UpdatePlayersBoard();
        }

        public override string ToString() => $"[MasterUpdatePlayerNameCommand, Player: {Player}, NewPlayerName: {NewPlayerName}]";
    }
}