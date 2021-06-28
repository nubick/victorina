using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class SelectRoundCommand : Command, IServerCommand
    {
        [Inject] private PlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
        public int RoundNumber { get; set; }
        
        public override CommandType Type => CommandType.SelectRound;
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.Round && 
                PlayStateData.Type != PlayStateType.Lobby &&
                PlayStateData.Type != PlayStateType.ShowAnswer)
            {
                Debug.Log($"Can't select round in PlayState: {PlayStateData.PlayState}");
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
                PlayStateSystem.ChangePlayState(new FinalRoundPlayState{Round = round});
                FinalRoundSystem.Reset();
            }
        }
        
        public override string ToString()
        {
            return $"[SelectRoundCommand, {nameof(RoundNumber)}:{RoundNumber}]";
        }
    }
}