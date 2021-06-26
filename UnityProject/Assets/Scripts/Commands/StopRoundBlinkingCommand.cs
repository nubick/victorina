using System;
using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class StopRoundBlinkingCommand : Command, IServerCommand
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }

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
            NetQuestion netQuestion = PackageSystem.BuildNetQuestion(blinkingPlayState.QuestionId);
            
            switch (netQuestion.Type)
            {
                case QuestionType.Auction:
                    AuctionPlayState auctionPlayState = new AuctionPlayState();
                    auctionPlayState.QuestionId = netQuestion.QuestionId;
                    PlayStateSystem.ChangePlayState(auctionPlayState);
                    AuctionSystem.StartNew(netQuestion.Price, netQuestion.GetTheme());
                    break;
                case QuestionType.CatInBag:
                    CatInBagPlayState catInBagPlayState = new CatInBagPlayState();
                    catInBagPlayState.NetQuestion = netQuestion;
                    PlayStateSystem.ChangePlayState(catInBagPlayState);
                    CommandsSystem.AddNewCommand(new PlaySoundEffectCommand {SoundId = SoundId.MeowIntro});
                    break;
                case QuestionType.NoRisk:
                    NoRiskPlayState noRiskPlayState = new NoRiskPlayState();
                    noRiskPlayState.QuestionId = netQuestion.QuestionId;
                    PlayStateSystem.ChangePlayState(noRiskPlayState);
                    break;
                case QuestionType.Simple:
                    PlayStateSystem.ChangeToShowQuestionPlayState(blinkingPlayState.QuestionId);
                    break;
                default:
                    throw new Exception($"Not supported QuestionType: {netQuestion.Type}");
            }
        }
        
        public override string ToString()
        {
            return "[StopRoundBlinkingCommand]";
        }
    }
}