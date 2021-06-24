using System;
using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class AcceptAnswerAsWrongCommand : Command, IServerCommand
    {
        public override CommandType Type => CommandType.AcceptAnswerAsWrong;
        
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        
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
            switch (PlayState.ShowQuestionPlayState.NetQuestion.Type)
            {
                case QuestionType.Simple:
                    QuestionAnswerData.WrongAnsweredIds.Add(PlayState.AnsweringPlayerId);
                    PlayersBoardSystem.FinePlayer(PlayState.AnsweringPlayerId, PlayState.Price);
                    PlayStateSystem.ChangeBackToShowQuestionPlayState(PlayState.ShowQuestionPlayState);
                    break;
                case QuestionType.NoRisk:
                    PlayStateSystem.ChangeToShowAnswerPlayState(PlayState.ShowQuestionPlayState);
                    break;
                case QuestionType.CatInBag:
                case QuestionType.Auction:
                    PlayersBoardSystem.FinePlayer(PlayState.AnsweringPlayerId, PlayState.Price);
                    PlayStateSystem.ChangeToShowAnswerPlayState(PlayState.ShowQuestionPlayState);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}