using System.Linq;
using Injection;

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
            MasterFilesRepository.AddMasterFiles(Data.Package);
        }

        public Question GetQuestion(string questionId)
        {
            var questions = Data.Package.Rounds.SelectMany(round => round.Themes.SelectMany(theme => theme.Questions));
            return questions.Single(question => question.Id == questionId);
        }
    }
}