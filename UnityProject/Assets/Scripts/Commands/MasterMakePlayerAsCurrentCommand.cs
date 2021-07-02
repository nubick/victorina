using Injection;
using UnityEngine;
using Victorina;
using Victorina.Commands;

namespace Commands
{
    public class MasterMakePlayerAsCurrentCommand : Command, IServerCommand
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }

        public byte PlayerId { get; }
        
        public override CommandType Type => CommandType.MasterMakePlayerAsCurrent;

        public MasterMakePlayerAsCurrentCommand(byte playerId)
        {
            PlayerId = playerId;
        }
        
        public bool CanExecuteOnServer()
        {
            if (PlayersBoardSystem.IsCurrentPlayer(PlayerId))
            {
                Debug.Log($"Cmd: Can't make player '{PlayerId}' as current. Is it current now.");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            PlayersBoardSystem.MakePlayerCurrent(PlayerId);
        }

        public override string ToString() => $"[MasterMakePlayerAsCurrentCommand, Player: {PlayerId}]";
    }
}