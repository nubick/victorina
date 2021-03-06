using System;
using System.Collections.Generic;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PackageSystem
    {
        [Inject] private PackageData Data { get; set; }
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }
        [Inject] private FilesDeliveryStatusManager FilesDeliveryStatusManager { get; set; }
        
        public void StartPackageGame(Package package)
        {
            Data.Package = package;
            WritePackageStatistics(Data.Package);
            Data.PackageProgress = new PackageProgress();
            MasterFilesRepository.AddPackageFiles(Data.Package);
            Data.NetRounds = BuildNetRounds(package);//After AddPackagePiles, ids for files should be generated
        }

        private void WritePackageStatistics(Package package)
        {
            int themesAmount = package.Rounds.Sum(round => round.Themes.Count);
            List<Question> questions = PackageTools.GetAllQuestions(package).ToList();
            int questionsAmount = questions.Count;
            int noRiskAmount = questions.Count(_ => _.Type == QuestionType.NoRisk);
            int catInBagAmount = questions.Count(_ => _.Type == QuestionType.CatInBag);
            int auctionAmount = questions.Count(_ => _.Type == QuestionType.Auction);
            Debug.Log($"Rounds: {package.Rounds.Count}, Themes: {themesAmount}, Questions: {questionsAmount}, NoRisk: {noRiskAmount}, CatInBag: {catInBagAmount}, Auctions: {auctionAmount}");
        }
        
        public Question GetQuestion(string questionId)
        {
            return PackageTools.GetAllQuestions(Data.Package).Single(question => question.Id == questionId);
        }

        public string GetTheme(string questionId)
        {
            Question question = GetQuestion(questionId);
            Theme theme = PackageTools.GetQuestionTheme(Data.Package, question);
            return theme.Name;
        }
        
        public NetRoundQuestion GetNetRoundQuestion(string questionId)
        {
            NetRoundQuestion netRoundQuestion = Data.NetRounds.SelectMany(round => round.Themes.SelectMany(theme => theme.Questions))
                .SingleOrDefault(question => question.QuestionId == questionId);

            if (netRoundQuestion == null)
                throw new Exception($"Can't find NetRoundQuestion by id:{questionId}");

            return netRoundQuestion;
        }

        public (int[], int[], int[]) GetPackageFilesInfo(Package package)
        {
            int roundNumber = 0;
            List<int> fileIds = new List<int>();
            List<int> chunksAmounts = new List<int>();
            List<int> priorities = new List<int>();
            foreach (Round round in package.Rounds)
            {
                roundNumber++;
                foreach (Question question in round.Themes.SelectMany(theme => theme.Questions))
                {
                    if(Data.PackageProgress.IsAnswered(question.Id))
                        continue;
                    
                    foreach (FileStoryDot fileStoryDot in GetFileStoryDots(question))
                    {
                        fileIds.Add(fileStoryDot.FileId);
                        chunksAmounts.Add(fileStoryDot.ChunksAmount);
                        int priority = roundNumber * 1000 + fileStoryDot.ChunksAmount;
                        priorities.Add(priority);
                    }
                }
            }
            return (fileIds.ToArray(), chunksAmounts.ToArray(), priorities.ToArray());
        }

        public List<FileStoryDot> GetFileStoryDots(Question question)
        {
            List<FileStoryDot> storyDots = new List<FileStoryDot>();
            storyDots.AddRange(question.QuestionStory.Where(_ => _ is FileStoryDot).Cast<FileStoryDot>());
            storyDots.AddRange(question.AnswerStory.Where(_ => _ is FileStoryDot).Cast<FileStoryDot>());
            return storyDots;
        }

        private List<NetRound> BuildNetRounds(Package package)
        {
            Data.RoundsMap = new Dictionary<Round, NetRound>();
            List<NetRound> netRounds = new List<NetRound>();
            foreach (Round round in package.Rounds)
            {
                NetRound netRound = BuildNetRound(round);
                netRounds.Add(netRound);
                Data.RoundsMap.Add(round, netRound);
            }
            return netRounds;
        }
        
        private NetRound BuildNetRound(Round round)
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
                    netRoundQuestion.Type = question.Type;
                    netRoundQuestion.FileIds = FilesDeliveryStatusManager.GetQuestionFileIds(question);
                    netRoundQuestion.IsDownloadedByMe = true;//Master has file from pack
                    netRoundTheme.Questions.Add(netRoundQuestion);
                }
                netRound.Themes.Add(netRoundTheme);
            }
            return netRound;
        }

        public NetRound GetNetRound(Round round, PackageProgress packageProgress)
        {
            NetRound netRound = Data.RoundsMap[round];
            var questions = netRound.Themes.SelectMany(theme => theme.Questions);
            foreach (NetRoundQuestion netRoundQuestion in questions)
            {
                netRoundQuestion.IsAnswered = packageProgress.IsAnswered(netRoundQuestion.QuestionId);
                netRoundQuestion.IsDownloadedByAll = FilesDeliveryStatusManager.IsDownloadedByAll(netRoundQuestion.FileIds);
            }
            return netRound;
        }
        
        public NetQuestion BuildNetQuestion(string questionId)
        {
            Question question = GetQuestion(questionId);
            NetQuestion netQuestion = new NetQuestion();
            netQuestion.QuestionId = questionId;
            netQuestion.Type = question.Type;
            netQuestion.Theme = GetTheme(questionId);
            netQuestion.Price = question.Price;
            netQuestion.QuestionStory = question.QuestionStory.ToArray();
            netQuestion.QuestionStoryDotsAmount = netQuestion.QuestionStory.Length;
            netQuestion.AnswerStory = question.AnswerStory.ToArray();
            netQuestion.AnswerStoryDotsAmount = netQuestion.AnswerStory.Length;
            netQuestion.CatInBagInfo = question.CatInBagInfo;
            return netQuestion;
        }
    }
}