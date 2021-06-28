using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class FinishNoRiskCommand : Command, IServerCommand
    {
        [Inject] private PlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private MatchData MatchData { get; set; }

        public override CommandType Type => CommandType.FinishNoRisk;
        private NoRiskPlayState PlayState => PlayStateData.As<NoRiskPlayState>();
        
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
            PlayStateSystem.ChangeToShowQuestionPlayState(MatchData.NetQuestion, MatchData.NetQuestion.Price);
        }

        public override string ToString() => "[FinishNoRiskCommand]";
    }
}