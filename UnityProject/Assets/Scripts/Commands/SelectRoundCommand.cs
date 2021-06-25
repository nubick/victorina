using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class SelectRoundCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackagePlayStateData PackagePlayStateData { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
        public int RoundNumber { get; set; }
        
        public override CommandType Type => CommandType.SelectRound;
        
        public bool CanExecuteOnServer()
        {
            if (PackagePlayStateData.Type != PlayStateType.Round && 
                PackagePlayStateData.Type != PlayStateType.Lobby &&
                PackagePlayStateData.Type != PlayStateType.ShowAnswer)
            {
                Debug.Log($"Can't select round in PlayState: {PackagePlayStateData.PlayState}");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            MatchData.RoundNumber = RoundNumber;
            Round round = PackageData.Package.Rounds[RoundNumber - 1];
            if (round.Type == RoundType.Simple)
            {
                RoundPlayState playState = new RoundPlayState();
                playState.RoundNumber = RoundNumber;
                playState.RoundTypes = PackageData.Package.Rounds.Select(_ => _.Type).ToArray();
                playState.NetRound = PackageSystem.GetNetRound(round, PackageData.PackageProgress);
                PlayStateSystem.ChangePlayState(playState);
            }
            else if (round.Type == RoundType.Final)
            {
                PlayStateSystem.ChangePlayState(new FinalRoundPlayState());
                FinalRoundSystem.Select(round);
            }
        }
        
        public override string ToString()
        {
            return $"[SelectRoundCommand, {nameof(RoundNumber)}:{RoundNumber}]";
        }
    }
}