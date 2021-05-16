using System.Collections.Generic;
using System.Linq;

namespace Victorina
{
    public static class PackageTools
    {
        public static IEnumerable<Question> GetAllQuestions(Package package)
        {
            return package.Rounds.SelectMany(round => round.Themes.SelectMany(theme => theme.Questions));
        }

        public static IEnumerable<StoryDot> GetAllStories(Package package)
        {
            return GetAllQuestions(package).SelectMany(question => question.GetAllStories());
        }
    }
}