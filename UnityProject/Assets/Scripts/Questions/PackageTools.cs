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

        public static IEnumerable<StoryDot> GetAllStories(Theme theme)
        {
            return theme.Questions.SelectMany(question => question.GetAllStories());
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

        public static IEnumerable<FileStoryDot> GetAllFileStoryDots(Theme theme)
        {
            return GetAllStories(theme).Where(storyDot => storyDot is FileStoryDot).Cast<FileStoryDot>();
        }

        public static IEnumerable<FileStoryDot> GetAllFileStoryDots(Question question)
        {
            return question.GetAllStories().Where(storyDot => storyDot is FileStoryDot).Cast<FileStoryDot>();
        }
    }
}