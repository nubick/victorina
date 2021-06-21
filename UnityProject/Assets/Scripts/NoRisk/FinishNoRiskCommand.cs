using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class FinishNoRiskCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        
        public override CommandType Type => CommandType.FinishNoRisk;
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.NoRisk)
            {
                Debug.Log($"Can't finish NoRisk in PlayState: {PlayStateData}");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            ShowQuestionPlayState showQuestionPlayState = new ShowQuestionPlayState();
            PlayStateSystem.ChangePlayState(showQuestionPlayState);
        }
    }
}