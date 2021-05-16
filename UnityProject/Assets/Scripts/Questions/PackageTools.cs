using System;
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

        public static IEnumerable<Theme> GetAllThemes(Package package)
        {
            return package.Rounds.SelectMany(round => round.Themes);
        }
        
        public static IEnumerable<StoryDot> GetAllStories(Package package)
        {
            return GetAllQuestions(package).SelectMany(question => question.GetAllStories());
        }

        public static void DeleteTheme(Package package, Theme theme)
        {
            Round round = GetThemeRound(package, theme);
            round.Themes.Remove(theme);
        }

        private static Round GetThemeRound(Package package, Theme theme)
        {
            foreach(Round round in package.Rounds)
                if (round.Themes.Contains(theme))
                    return round;

            throw new Exception($"Package '{package}' doesn't contain theme '{theme}'");
        }

        public static void DeleteQuestion(Package package, Question question)
        {
            Theme theme = GetQuestionTheme(package, question);
            theme.Questions.Remove(question);
        }

        private static Theme GetQuestionTheme(Package package, Question question)
        {
            foreach(Theme theme in GetAllThemes(package))
                if (theme.Questions.Contains(question))
                    return theme;

            throw new Exception($"Package '{package}' doesn't contain question '{question}'");
        }
    }
}