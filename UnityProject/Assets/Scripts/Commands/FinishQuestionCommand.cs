using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class FinishQuestionCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public override CommandType Type => CommandType.FinishQuestionCommand;
        private ShowAnswerPlayState ShowAnswerPlayState => PlayStateData.PlayState as ShowAnswerPlayState;
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.ShowAnswer)
            {
                Debug.Log($"Can finish question only when ShowAnswerPlayState: {PlayStateData.PlayState}");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            PackageData.PackageProgress.SetQuestionAsAnswered(ShowAnswerPlayState.NetRoundQuestion.QuestionId);
            RoundPlayState roundPlayState = PlayStateSystem.Get<RoundPlayState>();
            CommandsSystem.AddNewCommand(new SelectRoundCommand {RoundNumber = roundPlayState.RoundNumber});
        }
    }
}