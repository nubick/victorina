using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class SelectRoundCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        [Inject] private FilesDeliveryStatusManager FilesDeliveryStatusManager { get; set; }
        
        public int RoundNumber { get; set; }
        
        public override CommandType Type => CommandType.SelectRound;
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.Round && PlayStateData.Type != PlayStateType.Lobby)
            {
                Debug.Log($"Try to select round in PlayState: {PlayStateData.PlayState}");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            Round round = PackageData.Package.Rounds[RoundNumber - 1];
            if (round.Type == RoundType.Simple)
            {
                RoundPlayState playState = new RoundPlayState();
                playState.RoundNumber = RoundNumber;
                playState.RoundTypes = PackageData.Package.Rounds.Select(_ => _.Type).ToArray();
                playState.NetRound = BuildNetRound(round, PackageData.PackageProgress);
                PlayStateSystem.ChangePlayState(playState);
            }
            else if (round.Type == RoundType.Final)
            {
                PlayStateSystem.ChangePlayState(new FinalRoundPlayState());
                FinalRoundSystem.Select(round);
            }
        }
        
        private NetRound BuildNetRound(Round round, PackageProgress packageProgress)
        {
            NetRound netRound = new NetRound();
            foreach (Theme theme in round.Themes)
            {
                NetRoundTheme netRoundTheme = new NetRoundTheme();
                netRoundTheme.Name = theme.Name;
                foreach (Question question in theme.Questions)
                {
                    NetRoundQuestion netRoundQuestion = new NetRoundQuestion(question.Id);
                    netRoundQuestion.Price = question.Price;
                    netRoundQuestion.IsAnswered = packageProgress.IsAnswered(question.Id);
                    netRoundQuestion.Type = question.Type;
                    netRoundQuestion.Theme = theme.Name;
                    netRoundQuestion.IsDownloadedByMe = true;//Master has file from pack
                    netRoundQuestion.FileIds = FilesDeliveryStatusManager.GetQuestionFileIds(question);
                    netRoundQuestion.IsDownloadedByAll = FilesDeliveryStatusManager.IsDownloadedByAll(netRoundQuestion.FileIds);
                    netRoundTheme.Questions.Add(netRoundQuestion);
                }
                netRound.Themes.Add(netRoundTheme);
            }
            return netRound;
        }

        public override string ToString()
        {
            return $"[SelectRoundCommand, {nameof(RoundNumber)}:{RoundNumber}]";
        }
    }
}