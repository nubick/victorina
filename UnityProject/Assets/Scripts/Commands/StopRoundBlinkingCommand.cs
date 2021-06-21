using System;
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
        [Inject] private AuctionSystem AuctionSystem { get; set; }

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
            
            switch (netQuestion.Type)
            {
                case QuestionType.Auction:
                    AuctionPlayState auctionPlayState = new AuctionPlayState();
                    PlayStateSystem.ChangePlayState(auctionPlayState);
                    NetRoundQuestion question = PackageSystem.GetNetRoundQuestion(blinkingPlayState.QuestionId);
                    AuctionSystem.StartNew(question.Price, question.Theme);
                    break;
                case QuestionType.CatInBag:
                    CatInBagPlayState catInBagPlayState = new CatInBagPlayState();
                    catInBagPlayState.NetQuestion = netQuestion;
                    PlayStateSystem.ChangePlayState(catInBagPlayState);
                    break;
                case QuestionType.NoRisk:
                    NoRiskPlayState noRiskPlayState = new NoRiskPlayState();
                    PlayStateSystem.ChangePlayState(noRiskPlayState);
                    break;
                case QuestionType.Simple:
                    ShowQuestionPlayState showQuestionPlayState = new ShowQuestionPlayState();
                    PlayStateSystem.ChangePlayState(showQuestionPlayState);
                    break;
                default:
                    throw new Exception($"Not supported QuestionType: {netQuestion.Type}");
            }
            
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