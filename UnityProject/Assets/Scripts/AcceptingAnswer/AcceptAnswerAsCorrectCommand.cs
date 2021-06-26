using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class AcceptAnswerAsCorrectCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        
        public override CommandType Type => CommandType.AcceptAnswerAsCorrect;
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
            PlayersBoardSystem.MakePlayerCurrent(PlayState.AnsweringPlayerId);
            PlayersBoardSystem.RewardPlayer(PlayState.AnsweringPlayerId, PlayState.ShowQuestionPlayState.Price);
            PlayStateSystem.ChangeToShowAnswerPlayState(PlayState.ShowQuestionPlayState);
        }
    }
}