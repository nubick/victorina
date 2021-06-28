using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class ShowAnswerCommand : Command, IServerCommand
    {
        [Inject] private PlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        public override CommandType Type => CommandType.ShowAnswer;
        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.ShowQuestion)
            {
                Debug.Log($"Can't show answer in PlayState: {PlayStateData}");
                return false;
            }
            return true;
        }
        
        public void ExecuteOnServer()
        {
            PlayStateSystem.ChangeToShowAnswerPlayState(PlayState);
        }

        public override string ToString() => "[ShowAnswerCommand]";
    }
}