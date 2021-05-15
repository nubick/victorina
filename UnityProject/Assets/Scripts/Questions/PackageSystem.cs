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
        [Inject] private PackageFilesSystem PackageFilesSystem { get; set; }

        public void Initialize(string packagePath)
        {
            Data.Package = PackageFilesSystem.LoadPackage(packagePath);
            WritePackageStatistics(Data.Package);
            Data.PackageProgress = new PackageProgress();
            MasterFilesRepository.AddPackageFiles(Data.Package);
        }

        private void WritePackageStatistics(Package package)
        {
            int themesAmount = package.Rounds.Sum(round => round.Themes.Count);
            List<Question> questions = package.Rounds.SelectMany(round => round.Themes.SelectMany(theme => theme.Questions)).ToList();
            int questionsAmount = questions.Count;
            int noRiskAmount = questions.Count(_ => _.Type == QuestionType.NoRisk);
            int catInBagAmount = questions.Count(_ => _.Type == QuestionType.CatInBag);
            int auctionAmount = questions.Count(_ => _.Type == QuestionType.Auction);
            Debug.Log($"Rounds: {package.Rounds.Count}, Themes: {themesAmount}, Questions: {questionsAmount}, NoRisk: {noRiskAmount}, CatInBag: {catInBagAmount}, Auctions: {auctionAmount}");
        }
        
        public Question GetQuestion(string questionId)
        {
            var questions = Data.Package.Rounds.SelectMany(round => round.Themes.SelectMany(theme => theme.Questions));
            return questions.Single(question => question.Id == questionId);
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
        
    }
}