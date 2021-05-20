using System.Collections.Generic;
using System.IO;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class CrafterBagSystem
    {
        [Inject] private CrafterData Data { get; set; }
        [Inject] private PathData PathData { get; set; }
        [Inject] private PackageFilesSystem PackageFilesSystem { get; set; }
        
        public void RefreshBags()
        {
            Data.BagSelectedThemes.Clear();
            Data.BagAllThemes = LoadThemes();
        }

        private List<Theme> LoadThemes()
        {
            List<Theme> themes = new List<Theme>();
            string[] themesPaths = Directory.GetDirectories(PathData.CrafterBagPath);
            foreach (string themeFolderPath in themesPaths)
            {
                Theme theme = PackageFilesSystem.LoadTheme(themeFolderPath);
                themes.Add(theme);
            }
            return themes;
        }
        
        public void AddSelectedThemesToRound()
        {
            Debug.Log($"Add {Data.BagSelectedThemes.Count} to round: {Data.SelectedRound.Name}");
            foreach (Theme theme in Data.BagSelectedThemes)
            {
                PackageFilesSystem.CopyFiles(theme, Data.SelectedPackage);
                PackageFilesSystem.FillFilePaths(theme, Data.SelectedPackage.Path);
                Data.SelectedRound.Themes.Add(theme);
                PackageFilesSystem.UpdatePackageJson(Data.SelectedPackage);
            }
        }
    }
}