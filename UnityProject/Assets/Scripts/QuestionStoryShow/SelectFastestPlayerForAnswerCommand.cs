using System.Linq;
using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class SelectFastestPlayerForAnswerCommand : Command, IServerCommand
    {
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        public override CommandType Type => CommandType.SelectFastestPlayerForAnswer;
        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.ShowQuestion)
            {
                Debug.Log($"Can't execute when PlayState: {PlayStateData}");
                return false;
            }

            if (PlayersButtonClickData.Players.Count == 0)
            {
                Debug.Log($"Can't execute, players list is empty.");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            PlayerButtonClickData fastest = PlayersButtonClickData.Players.OrderBy(_ => _.Time).First();
            PlayStateSystem.ChangeToAcceptingAnswerPlayState(PlayState, fastest.PlayerId);
        }
    }
}