using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class FinishQuestionCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateData PackagePlayStateData { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
        public override CommandType Type => CommandType.FinishQuestion;
        private ShowAnswerPlayState ShowAnswerPlayState => PackagePlayStateData.PlayState as ShowAnswerPlayState;
        
        public bool CanExecuteOnServer()
        {
            if (PackagePlayStateData.Type != PlayStateType.ShowAnswer)
            {
                Debug.Log($"Can't finish question when: {PackagePlayStateData.PlayState}");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            PackageData.PackageProgress.SetQuestionAsAnswered(ShowAnswerPlayState.NetQuestion.QuestionId);
            CommandsSystem.AddNewCommand(new SelectRoundCommand {RoundNumber = MatchData.RoundNumber});
        }

        public override string ToString() => "[FinishQuestionCommand]";
    }
}