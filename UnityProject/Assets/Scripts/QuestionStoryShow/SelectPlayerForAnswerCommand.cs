using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class SelectPlayerForAnswerCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        
        public byte PlayerId { get; set; }

        public override CommandType Type => CommandType.SelectPlayerForAnswer;
        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.ShowQuestion)
            {
                Debug.Log($"Can't execute when PlayState: {PlayStateData}");
                return false;
            }
            return true;
        }

        public void ExecuteOnServer()
        {
            PlayStateSystem.ChangeToAcceptingAnswerPlayState(PlayState, PlayerId);
        }
    }
}