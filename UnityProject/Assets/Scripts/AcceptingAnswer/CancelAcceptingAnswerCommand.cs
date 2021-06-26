using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class CancelAcceptingAnswerCommand : Command, IServerCommand
    {
        public override CommandType Type => CommandType.CancelAcceptingAnswer;
        
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        private AcceptingAnswerPlayState PlayState => PlayStateData.As<AcceptingAnswerPlayState>();
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.AcceptingAnswer)
            {
                Debug.Log($"Can't accept answer in PlayState: {PlayStateData}");
                return false;
            }
            return true;
        }

        public void ExecuteOnServer()
        {
            PlayStateSystem.ChangeBackToShowQuestionPlayState(PlayState.ShowQuestionPlayState);
        }
    }
}