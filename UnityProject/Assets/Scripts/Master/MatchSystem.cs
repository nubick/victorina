using System;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class MatchSystem
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private SendToMasterService SendToMasterService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }

        public void Start()
        {
            SelectRound(1);
        }

        public void TrySelectQuestion(NetRoundQuestion netRoundQuestion)
        {
            if (netRoundQuestion.IsAnswered)
            {
                Debug.Log($"Selected question is answered: {netRoundQuestion}");
            }
            else if (NetworkData.IsMaster)
            {
                SelectQuestion(netRoundQuestion);
            }
            else // if (NetworkData.IsClient)
            {
                if (IsCurrentPlayer(NetworkData.PlayerId))
                {
                    Debug.Log($"Player: Current player selected round question: {netRoundQuestion}");
                    SendToMasterService.SendSelectRoundQuestion(netRoundQuestion);
                }
                else
                {
                    Debug.Log($"Only Master or Current player can select question: {netRoundQuestion}");
                }
            }
        }

        public bool IsCurrentPlayer(ulong playerId)
        {
            return MatchData.PlayersBoard.Value.Current != null && MatchData.PlayersBoard.Value.Current.Id == playerId;
        }

        public void SelectQuestion(NetRoundQuestion netRoundQuestion)
        {
            MatchData.SelectedRoundQuestion = netRoundQuestion;
            SendToPlayersService.SendSelectedRoundQuestion(MatchData.SelectedRoundQuestion);

            MatchData.Phase.Value = MatchPhase.Question;
            SendToPlayersService.SendMatchPhase(MatchData.Phase.Value);

            NetQuestion netQuestion = BuildNetQuestion(netRoundQuestion);
            QuestionAnswerSystem.StartAnswer(netQuestion);
        }

        private NetQuestion BuildNetQuestion(NetRoundQuestion netRoundQuestion)
        {
            Question question = PackageSystem.GetQuestion(netRoundQuestion.QuestionId);
            NetQuestion netQuestion = new NetQuestion();
            netQuestion.QuestionStory = question.QuestionStory.ToArray();
            netQuestion.QuestionStoryDotsAmount = netQuestion.QuestionStory.Length;
            netQuestion.AnswerStory = question.AnswerStory.ToArray();
            netQuestion.AnswerStoryDotsAmount = netQuestion.AnswerStory.Length;
            return netQuestion;
        }
        
        public void BackToRound()
        {
            PackageData.PackageProgress.SetQuestionAsAnswered(MatchData.SelectedRoundQuestion.QuestionId);
            SelectRound(MatchData.RoundsInfo.Value.CurrentRoundNumber );
        }

        public void SelectRound(int number)
        {
            if (NetworkData.IsClient)
                return;

            MatchData.RoundsInfo.Value.RoundsAmount = PackageData.Package.Rounds.Count;
            MatchData.RoundsInfo.Value.CurrentRoundNumber = number;
            SendToPlayersService.SendNetRoundsInfo(MatchData.RoundsInfo.Value);

            Round round = PackageData.Package.Rounds[number - 1];
            MatchData.RoundData.Value = BuildNetRound(round, PackageData.PackageProgress);
            SendToPlayersService.SendNetRound(MatchData.RoundData.Value);

            MatchData.Phase.Value = MatchPhase.Round;
            SendToPlayersService.SendMatchPhase(MatchData.Phase.Value);

            (int[] fileIds, int[] chunksAmounts) info = PackageSystem.GetRoundFileIds(round);
            SendToPlayersService.SendRoundFileIds(info.fileIds, info.chunksAmounts);
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
                    netRoundTheme.Questions.Add(netRoundQuestion);
                }
                netRound.Themes.Add(netRoundTheme);
            }
            return netRound;
        }

        public void RewardPlayer(ulong playerId)
        {
            PlayerData player = GetPlayer(playerId);
            player.Score += MatchData.SelectedRoundQuestion.Price;
            Debug.Log($"Reward player '{playerId}':'{player.Name}' by {MatchData.SelectedRoundQuestion.Price}, new score: {player.Score}");
            MatchData.PlayersBoard.NotifyChanged();
            SendToPlayersService.Send(MatchData.PlayersBoard.Value);
        }

        public void FinePlayer(ulong playerId)
        {
            PlayerData player = GetPlayer(playerId);
            player.Score -= MatchData.SelectedRoundQuestion.Price;
            Debug.Log($"Fine player '{playerId}':'{player.Name}' by {MatchData.SelectedRoundQuestion.Price}, new score: {player.Score}");
            MatchData.PlayersBoard.NotifyChanged();
            SendToPlayersService.Send(MatchData.PlayersBoard.Value);
        }

        public PlayerData GetPlayer(ulong playerId)
        {
            PlayerData player = MatchData.PlayersBoard.Value.Players.SingleOrDefault(_ => _.Id == playerId);
            if (player == null)
                throw new Exception($"Can't find player with id: {playerId}");
            return player;
        }
    }
}