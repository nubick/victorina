using System.Collections.Generic;
using System.Linq;
using Injection;
using UnityEngine;
using UnityEngine.Animations;

namespace Victorina
{
    public class PackageSystem
    {
        [Inject] private PackageData Data { get; set; }
        [Inject] private SiqConverter SiqConverter { get; set; }
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }

        public void Initialize(string packageName)
        {
            Data.Package = SiqConverter.Convert(packageName);
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
            Debug.Log($"Rounds: {package.Rounds.Count}, Themes: {themesAmount}, Questions: {questionsAmount}, NoRisk: {noRiskAmount}");
        }
        
        public Question GetQuestion(string questionId)
        {
            var questions = Data.Package.Rounds.SelectMany(round => round.Themes.SelectMany(theme => theme.Questions));
            return questions.Single(question => question.Id == questionId);
        }

        public (int[], int[]) GetRoundFileIds(Round round)
        {
            List<FileStoryDot> fileStoryDots = new List<FileStoryDot>();
            foreach (Question question in round.Themes.SelectMany(theme => theme.Questions))
            {
                fileStoryDots.AddRange(GetFileStoryDots(question.QuestionStory));
                fileStoryDots.AddRange(GetFileStoryDots(question.AnswerStory));
            }

            int[] fileIds = new int[fileStoryDots.Count];
            int[] chunksAmounts = new int[fileStoryDots.Count];

            for (int i = 0; i < fileStoryDots.Count; i++)
            {
                fileIds[i] = fileStoryDots[i].FileId;
                chunksAmounts[i] = fileStoryDots[i].ChunksAmount;
            }

            return (fileIds, chunksAmounts);
        }

        private List<FileStoryDot> GetFileStoryDots(List<StoryDot> story)
        {
            return story.Where(_ => _ is FileStoryDot).Cast<FileStoryDot>().ToList();
        }
        
    }
}