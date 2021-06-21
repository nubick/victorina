using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class StopRoundBlinkingCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }

        public override CommandType Type => CommandType.StopRoundBlinking;
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.RoundBlinking)
            {
                Debug.Log($"Can stop round blinking only at RoundBlinkingPlayState, current play state: {PlayStateData}");
                return false;
            }
            return true;
        }

        public void ExecuteOnServer()
        {
            RoundBlinkingPlayState blinkingPlayState = PlayStateData.As<RoundBlinkingPlayState>();
            NetQuestion netQuestion = BuildNetQuestion(blinkingPlayState.QuestionId);

            ShowQuestionPlayState newPlayState = new ShowQuestionPlayState();
            PlayStateSystem.ChangePlayState(newPlayState);
            
            QuestionAnswerSystem.StartAnswer(netQuestion);
        }
        
        private NetQuestion BuildNetQuestion(string questionId)
        {
            Question question = PackageSystem.GetQuestion(questionId);
            NetQuestion netQuestion = new NetQuestion();
            netQuestion.Type = question.Type;
            netQuestion.QuestionStory = question.QuestionStory.ToArray();
            netQuestion.QuestionStoryDotsAmount = netQuestion.QuestionStory.Length;
            netQuestion.AnswerStory = question.AnswerStory.ToArray();
            netQuestion.AnswerStoryDotsAmount = netQuestion.AnswerStory.Length;
            netQuestion.CatInBagInfo = question.CatInBagInfo;
            return netQuestion;
        }

        public override string ToString()
        {
            return "[StopRoundBlinkingCommand]";
        }
    }
}