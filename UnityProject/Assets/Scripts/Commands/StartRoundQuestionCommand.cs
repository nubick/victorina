using System;
using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class StartRoundQuestionCommand : Command, IServerCommand
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }

        public override CommandType Type => CommandType.StartRoundQuestion;
        private RoundPlayState PlayState => PlayStateData.As<RoundPlayState>();
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.Round)
            {
                Debug.Log($"Can't start round question from play state: {PlayStateData}");
                return false;
            }

            if (PlayState.SelectedQuestionId == null)
            {
                Debug.Log("Can't start question. Selected question id is null.");
                return false;
            }
            
            return true;
        }

        public void ExecuteOnServer()
        {
            NetQuestion netQuestion = PackageSystem.BuildNetQuestion(PlayState.SelectedQuestionId);
            MatchData.NetQuestion = netQuestion;
            
            switch (netQuestion.Type)
            {
                case QuestionType.Auction:
                    AuctionPlayState auctionPlayState = new AuctionPlayState();
                    PlayStateSystem.ChangePlayState(auctionPlayState);
                    AuctionSystem.StartNew(netQuestion);
                    break;
                case QuestionType.CatInBag:
                    CatInBagPlayState catInBagPlayState = new CatInBagPlayState();
                    catInBagPlayState.NetQuestion = netQuestion;
                    PlayStateSystem.ChangePlayState(catInBagPlayState);
                    ServerEvents.PlaySoundEffect.Publish(SoundId.MeowIntro.ToString());
                    break;
                case QuestionType.NoRisk:
                    NoRiskPlayState noRiskPlayState = new NoRiskPlayState();
                    PlayStateSystem.ChangePlayState(noRiskPlayState);
                    break;
                case QuestionType.Simple:
                    PlayStateSystem.ChangeToShowQuestionPlayState(netQuestion, netQuestion.Price);
                    break;
                default:
                    throw new Exception($"Not supported QuestionType: {netQuestion.Type}");
            }
        }
        
        public override string ToString()
        {
            return "[StartRoundQuestionCommand]";
        }
    }
}