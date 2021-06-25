using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class FinishQuestionCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackagePlayStateData PackagePlayStateData { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public override CommandType Type => CommandType.FinishQuestion;
        private ShowAnswerPlayState ShowAnswerPlayState => PackagePlayStateData.PlayState as ShowAnswerPlayState;
        
        public bool CanExecuteOnServer()
        {
            if (PackagePlayStateData.Type != PlayStateType.ShowAnswer)
            {
                Debug.Log($"Can finish question only when ShowAnswerPlayState: {PackagePlayStateData.PlayState}");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            PackageData.PackageProgress.SetQuestionAsAnswered(ShowAnswerPlayState.NetQuestion.QuestionId);
            RoundPlayState roundPlayState = new RoundPlayState();//todo: bind round play state
            CommandsSystem.AddNewCommand(new SelectRoundCommand {RoundNumber = roundPlayState.RoundNumber});
        }
    }
}