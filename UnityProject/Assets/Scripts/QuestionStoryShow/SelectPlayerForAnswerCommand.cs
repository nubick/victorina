using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class SelectPlayerForAnswerCommand : Command, IServerCommand
    {
        [Inject] private PlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        public byte PlayerId { get; }

        public override CommandType Type => CommandType.SelectPlayerForAnswer;
        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();

        public SelectPlayerForAnswerCommand(byte playerId)
        {
            PlayerId = playerId;
        }
        
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

        public override string ToString() => $"[SelectPlayerForAnswerCommand, playerId: {PlayerId}]";
    }
}