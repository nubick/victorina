using System.Linq;
using Injection;

namespace Victorina
{
    public class PackageSystem
    {
        [Inject] private PackageData Data { get; set; }

        public void Initialize()
        {
            SiConverter siConverter = new SiConverter();
            Data.Package = siConverter.Convert();;
        }

        public Question GetQuestion(string questionId)
        {
            var questions = Data.Package.Rounds.SelectMany(round => round.Themes.SelectMany(theme => theme.Questions));
            return questions.Single(question => question.Id == questionId);
        }
    }
}