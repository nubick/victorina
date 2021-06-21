using System;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PackagePlayStateSystem
    {
        [Inject] private PackagePlayStateData Data { get; set; }
        
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
            return playStateType switch
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
    }
}