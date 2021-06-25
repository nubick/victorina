using System;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PackagePlayStateSystem
    {
        private Injector _injector;
        
        [Inject] private PackagePlayStateData Data { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        
        public void Initialize(Injector injector)
        {
            _injector = injector;
        }
        
        public void StartPackageOnLobby()
        {
            ChangePlayState(new LobbyPlayState());
        }

        public void ChangePlayState(PackagePlayState playState)
        {
            Data.PlayState = playState;
            Data.MarkAsChanged();
            Debug.Log($"CHANGE playState: {playState}");
        }

        public PackagePlayState Create(PlayStateType playStateType)
        {
            PackagePlayState playState = playStateType switch
            {
                PlayStateType.Lobby => new LobbyPlayState(),

                PlayStateType.Round => new RoundPlayState(),
                PlayStateType.RoundBlinking => new RoundBlinkingPlayState(),
                PlayStateType.FinalRound => new FinalRoundPlayState(),

                PlayStateType.Auction => new AuctionPlayState(),
                PlayStateType.CatInBag => new CatInBagPlayState(),
                PlayStateType.NoRisk => new NoRiskPlayState(),

                PlayStateType.ShowQuestion => new ShowQuestionPlayState(),
                PlayStateType.AcceptingAnswer => new AcceptingAnswerPlayState(),
                PlayStateType.ShowAnswer => new ShowAnswerPlayState(),

                _ => throw new Exception($"Not supported PlayStateType: {playStateType}")
            };
            
            _injector.InjectTo(playState);
            return playState;
        }
        
        public void ChangeBackToShowQuestionPlayState(ShowQuestionPlayState showQuestionPlayState)
        {
            ChangePlayState(showQuestionPlayState);
            
            //todo: finish refactoring
            //StartTimer();
            //SendData(MasterIntention.ContinueTimer);
        }
        
        public void ChangeToShowAnswerPlayState(ShowQuestionPlayState showQuestionPlayState)
        {
            ShowAnswerPlayState playState = new ShowAnswerPlayState();
            playState.NetQuestion = showQuestionPlayState.NetQuestion;
            
            ChangePlayState(playState);
            PlayersButtonClickData.Clear();
            
            //todo: finish refactoring
            //Data.TimerState = QuestionTimerState.Paused;
        }

        public void ChangeToAcceptingAnswerPlayState(ShowQuestionPlayState showQuestionPlayState, byte playerId)
        {
            AcceptingAnswerPlayState playState = new AcceptingAnswerPlayState();
            playState.ShowQuestionPlayState = showQuestionPlayState;
            playState.Price = showQuestionPlayState.NetQuestion.Price;
            playState.AnsweringPlayerId = playerId;
            
            ChangePlayState(playState);
            PlayersButtonClickData.Clear();
        }
    }
}