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
            RegisterIfNew(playState);
            Data.PlayState = playState;
            Data.MarkAsChanged();
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
                PlayStateType.ShowAnswer => new ShowAnswerPlayState(),

                _ => throw new Exception($"Not supported PlayStateType: {playStateType}")
            };
            
            _injector.InjectTo(playState);
            return playState;
        }

        private void RegisterIfNew(PackagePlayState playState)
        {
            Type type = playState.GetType();
            if (Data.PlayStatesMap.ContainsKey(type))
            {
//                if (playState != Data.PlayStatesMap[type])
//                    throw new Exception($"PlayState duplicate was detected: {playState}");
            }
            else
            {
                Data.PlayStatesMap.Add(type, playState);
                Debug.Log($"REGISTER PlayState: {playState}");
            }
        }
        
        public T Get<T>() where T : PackagePlayState
        {
            Type type = typeof(T);
            return Data.PlayStatesMap.ContainsKey(type) ? 
                Data.PlayStatesMap[type] as T : 
                throw new Exception($"Can't find PlayState of type: {type}");
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