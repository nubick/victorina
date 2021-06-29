using Injection;
using UnityEngine;
using Victorina;
using Victorina.Commands;

namespace Commands
{
    public class MasterMakePlayerAsCurrentCommand : Command, IServerCommand
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }

        public PlayerData Player { get; set; }
        
        public override CommandType Type => CommandType.MasterMakePlayerAsCurrent;

        public bool CanExecuteOnServer()
        {
            if (PlayersBoardSystem.IsCurrentPlayer(Player))
            {
                Debug.Log($"Cmd: Can't make player '{Player}' as current. Is it current now.");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            PlayersBoardSystem.MakePlayerCurrent(Player);
        }

        public override string ToString() => $"[MasterMakePlayerAsCurrentCommand, Player: {Player}]";
    }
}