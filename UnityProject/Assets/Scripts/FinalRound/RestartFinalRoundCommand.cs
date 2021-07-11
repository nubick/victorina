using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class RestartFinalRoundCommand : Command, IServerCommand
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        
        public override CommandType Type => CommandType.RestartFinalRound;
        
        public bool CanExecuteOnServer()
        {
            return PlayStateData.Type == PlayStateType.FinalRound;
        }

        public void ExecuteOnServer()
        {
            FinalRoundSystem.Reset();
        }

        public override string ToString()
        {
            return "[RestartFinalRoundCommand]";
        }
    }
}