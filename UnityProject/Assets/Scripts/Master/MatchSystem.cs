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
        [Inject] private FilesDeliveryStatusManager FilesDeliveryStatusManager { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }

        private PlayersBoard PlayersBoard => MatchData.PlayersBoard.Value;
        
        public void Initialize()
        {
            MetagameEvents.ServerStarted.Subscribe(OnServerStarted);
        }

        private void OnServerStarted()
        {
            MatchData.Clear();
        }
        
        public void StartMatch()
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
            else if (NetworkData.IsClient)
            {
                if (MatchData.IsMeCurrentPlayer)
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

        public bool IsCurrentPlayer(byte playerId)
        {
            return PlayersBoard.Current != null && PlayersBoard.Current.PlayerId == playerId;
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
            netQuestion.Type = netRoundQuestion.Type;
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
            
            SyncCurrentRound();
            
            MatchData.Phase.Value = MatchPhase.Round;
            SendToPlayersService.SendMatchPhase(MatchData.Phase.Value);
        }

        public void SyncCurrentRound()
        {
            int number = MatchData.RoundsInfo.Value.CurrentRoundNumber;
            Round round = PackageData.Package.Rounds[number - 1];
            MatchData.RoundData.Value = BuildNetRound(round, PackageData.PackageProgress);
            SendToPlayersService.SendNetRound(MatchData.RoundData.Value);
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

        public void RewardPlayer(byte playerId)
        {
            int price = GetQuestionPrice(MatchData.QuestionAnswerData.SelectedQuestion.Value, MatchData.SelectedRoundQuestion);
            PlayerData player = PlayersBoardSystem.GetPlayer(playerId);
            player.Score += price;
            Debug.Log($"Reward player '{playerId}':'{player.Name}' by {MatchData.SelectedRoundQuestion.Price}, new score: {player.Score}");
            MatchData.PlayersBoard.NotifyChanged();
            SendToPlayersService.Send(PlayersBoard);
        }
        
        public void FinePlayer(byte playerId)
        {
            int price = GetQuestionPrice(MatchData.QuestionAnswerData.SelectedQuestion.Value, MatchData.SelectedRoundQuestion);
            PlayerData player = PlayersBoardSystem.GetPlayer(playerId);
            player.Score -= price;
            Debug.Log($"Fine player '{playerId}':'{player.Name}' by {MatchData.SelectedRoundQuestion.Price}, new score: {player.Score}");
            MatchData.PlayersBoard.NotifyChanged();
            SendToPlayersService.Send(PlayersBoard);
        }
        
        private int GetQuestionPrice(NetQuestion netQuestion, NetRoundQuestion netRoundQuestion)
        {
            if (netQuestion.Type == QuestionType.CatInBag)
            {
                CatInBagStoryDot catInBagStoryDot = netQuestion.GetFirst<CatInBagStoryDot>();

                if (catInBagStoryDot == null)
                    throw new Exception("Cat in bag netQuestion doesn't have CatInBagStoryDot at first place.");

                return catInBagStoryDot.Price;
            }
            else if (netQuestion.Type == QuestionType.Auction)
            {
                return MatchData.QuestionAnswerData.AuctionData.Value.Bet;
            }
            else
            {
                return netRoundQuestion.Price;
            }
        }
    }
}