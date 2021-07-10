using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class FinishFinalRoundCommand : Command, IServerCommand
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        
        public override CommandType Type => CommandType.FinishFinalRound;
        public bool CanExecuteOnServer() => PlayStateData.Type == PlayStateType.FinalRound;

        public void ExecuteOnServer()
        {
            MatchSystem.NavigateToNextRound();
        }

        public override string ToString()
        {
            return "[FinishFinalRoundCommand]";
        }
    }
}