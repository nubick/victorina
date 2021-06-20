using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class StopRoundBlinkingCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateData PackagePlayStateData { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        
        public override CommandType Type => CommandType.StopRoundBlinking;
        
        public bool CanExecuteOnServer()
        {
            if (PackagePlayStateData.Type != PlayStateType.RoundBlinking)
            {
                Debug.Log($"Can stop round blinking only at RoundBlinkingPlayState, current play state: {PackagePlayStateData}");
                return false;
            }
            return true;
        }

        public void ExecuteOnServer()
        {
            RoundBlinkingPlayState state = PackagePlayStateData.PlayState as RoundBlinkingPlayState;
            NetQuestion netQuestion = BuildNetQuestion(state.NetRoundQuestion);
            QuestionAnswerSystem.StartAnswer(netQuestion);
        }
        
        private NetQuestion BuildNetQuestion(NetRoundQuestion netRoundQuestion)
        {
            Question question = PackageSystem.GetQuestion(netRoundQuestion.QuestionId);
            NetQuestion netQuestion = new NetQuestion();
            netQuestion.Type = netRoundQuestion.Type;
            netQuestion.QuestionStory = question.QuestionStory.ToArray();
            netQuestion.QuestionStoryDotsAmount = netQuestion.QuestionStory.Length;
            netQuestion.AnswerStory = question.AnswerStory.ToArray();
            netQuestion.AnswerStoryDotsAmount = netQuestion.AnswerStory.Length;
            netQuestion.CatInBagInfo = question.CatInBagInfo;
            return netQuestion;
        }
    }
}