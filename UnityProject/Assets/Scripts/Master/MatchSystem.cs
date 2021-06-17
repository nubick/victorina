using System;
using System.Linq;
using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class MatchSystem
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private FilesDeliveryStatusManager FilesDeliveryStatusManager { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private MessageDialogueView MessageDialogueView { get; set; }
        
        private NetRoundsInfo RoundsInfo => MatchData.RoundsInfo.Value; 
        
        public void Initialize()
        {
            MetagameEvents.ServerStarted.Subscribe(OnServerStarted);
        }

        private void OnServerStarted()
        {
            MatchData.Clear();
            PlayersBoard.Clear();
        }
        
        public void StartMatch()
        {
            SelectRound(1);
        }

        public void TrySelectQuestion(NetRoundQuestion netRoundQuestion)
        {
            if (NetworkData.IsMaster && PlayersBoard.Current == null)
            {
                MessageDialogueView.Show("Текущий игрок?", "Для старта необходим текущий игрок! Выберите игрока в верхней панели и сделайте его текущим!");
                return;
            }
            
            CommandsSystem.AddNewCommand(new SelectRoundQuestionCommand(netRoundQuestion));
        }

        public bool IsCurrentPlayer(byte playerId)
        {
            return PlayersBoard.Current != null && PlayersBoard.Current.PlayerId == playerId;
        }

        public bool IsCurrentPlayer(PlayerData player)
        {
            return IsCurrentPlayer(player.PlayerId);
        }

        public void SelectQuestion(NetRoundQuestion netRoundQuestion)
        {
            SendSelectQuestionEvents();
            
            MatchData.SelectedRoundQuestion = netRoundQuestion;
            SendToPlayersService.SendSelectedRoundQuestion(MatchData.SelectedRoundQuestion);
            
            MatchData.Phase.Value = MatchPhase.Question;
            SendToPlayersService.SendMatchPhase(MatchData.Phase.Value);

            NetQuestion netQuestion = BuildNetQuestion(netRoundQuestion);
            QuestionAnswerSystem.StartAnswer(netQuestion);
        }

        private void SendSelectQuestionEvents()
        {
            int answered = MatchData.RoundData.Value.Themes.SelectMany(theme => theme.Questions).Count(question => question.IsAnswered);
            int notAnswered = MatchData.RoundData.Value.Themes.SelectMany(theme => theme.Questions).Count(question => !question.IsAnswered);

            if (answered == 0)
                AnalyticsEvents.FirstRoundQuestionStart.Publish(RoundsInfo.CurrentRoundNumber);
            else if (notAnswered == 1)
                AnalyticsEvents.LastRoundQuestionStart.Publish(RoundsInfo.CurrentRoundNumber);
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
            SelectRound(RoundsInfo.CurrentRoundNumber );
        }

        public void SelectRound(int number)
        {
            if (NetworkData.IsClient)
                return;

            RoundsInfo.RoundsAmount = PackageData.Package.Rounds.Count;
            RoundsInfo.CurrentRoundNumber = number;
            RoundsInfo.RoundTypes = PackageData.Package.Rounds.Select(_ => _.Type).ToArray();
            SendToPlayersService.SendNetRoundsInfo(RoundsInfo);

            Round round = PackageData.Package.Rounds[RoundsInfo.CurrentRoundNumber - 1];
            if (round.Type == RoundType.Simple)
                SyncSimpleRound();
            else if (round.Type == RoundType.Final)
                FinalRoundSystem.Select(round);

            MatchData.Phase.Value = MatchPhase.Round;
            SendToPlayersService.SendMatchPhase(MatchData.Phase.Value);
        }
        
        public void SyncSimpleRound()
        {
            Round round = PackageData.Package.Rounds[RoundsInfo.CurrentRoundNumber - 1];
            if (round.Type == RoundType.Simple)
            {
                MatchData.RoundData.Value = BuildNetRound(round, PackageData.PackageProgress);
                SendToPlayersService.SendNetRound(MatchData.RoundData.Value);
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

        public void RewardPlayer(byte playerId)
        {
            int price = GetQuestionPrice(MatchData.QuestionAnswerData.SelectedQuestion.Value, MatchData.SelectedRoundQuestion);
            PlayerData player = PlayersBoardSystem.GetPlayer(playerId);
            RewardPlayer(player, price);
        }

        public void RewardPlayer(PlayerData player, int price)
        {
            player.Score += price;
            Debug.Log($"Reward player {player} by {price}, new score: {player.Score}");
            PlayersBoard.MarkAsChanged();
        }
        
        public void FinePlayer(byte playerId)
        {
            int price = GetQuestionPrice(MatchData.QuestionAnswerData.SelectedQuestion.Value, MatchData.SelectedRoundQuestion);
            PlayerData player = PlayersBoardSystem.GetPlayer(playerId);
            FinePlayer(player, price);
        }

        public void FinePlayer(PlayerData player, int price)
        {
            player.Score -= price;
            Debug.Log($"Fine player {player} by {price}, new score: {player.Score}");
            PlayersBoard.MarkAsChanged();
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