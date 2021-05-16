using System.Collections.Generic;
using System.IO;
using System.Linq;
using Injection;

namespace Victorina
{
    public class CrafterBagSystem
    {
        [Inject] private PackageCrafterData Data { get; set; }
        [Inject] private PathData PathData { get; set; }
        [Inject] private PackageJsonConverter PackageJsonConverter { get; set; }
        
        public void RefreshBags()
        {
            Data.BagSelectedThemes.Clear();
            Data.BagAllThemes = LoadThemes();
        }

        private List<Theme> LoadThemes()
        {
            List<Theme> themes = new List<Theme>();
            string[] themesPaths = Directory.GetDirectories(PathData.CrafterBagPath);
            foreach (string themePath in themesPaths)
            {
                string themeJson = File.ReadAllText($"{themePath}/theme.json");
                Theme theme = PackageJsonConverter.ReadTheme(themeJson);
                FillFilePaths(theme, themePath);
                themes.Add(theme);
            }
            return themes;
        }
        
        private void FillFilePaths(Theme theme, string themePath)
        {
            var allStories = theme.Questions.SelectMany(question => question.GetAllStories());
            foreach (StoryDot storyDot in allStories)
            {
                if (storyDot is FileStoryDot fileStoryDot)
                    fileStoryDot.Path = $"{themePath}/{fileStoryDot.FileName}";
            }
        }
    }
}