using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class SelectRoundCommand : Command, IServerCommand
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }

        public int RoundNumber { get; }
        
        public override CommandType Type => CommandType.SelectRound;

        public SelectRoundCommand(int roundNumber)
        {
            RoundNumber = roundNumber;
        }
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.Round && 
                PlayStateData.Type != PlayStateType.Lobby &&
                PlayStateData.Type != PlayStateType.ShowAnswer)
            {
                Debug.Log($"Can't select round in PlayState: {PlayStateData.PlayState}");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
           MatchSystem.NavigateToRound(RoundNumber);
        }
        
        public override string ToString()
        {
            return $"[SelectRoundCommand, {nameof(RoundNumber)}:{RoundNumber}]";
        }
    }
}