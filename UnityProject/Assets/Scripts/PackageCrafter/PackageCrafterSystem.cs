using Injection;
using UnityEngine;

namespace Victorina
{
    public class PackageCrafterSystem
    {
        [Inject] private CrafterData Data { get; set; }
        [Inject] private PackageFilesSystem PackageFilesSystem { get; set; }
        [Inject] private PathData PathData { get; set; }

        public void LoadPackages()
        {
            Data.Packages.Clear();
            foreach (string packagePath in PackageFilesSystem.GetCrafterPackagesPaths())
                LoadPackage(packagePath);
            SelectPackage(null);
        }

        private void LoadPackage(string packagePath)
        {
            Package package = PackageFilesSystem.LoadPackage(packagePath);
            Data.Packages.Add(package);
        }
        
        public void SelectPackage(Package package)
        {
            Data.SelectedPackage = package;
            Data.SelectedRound = null;
            Data.SelectedTheme = null;
            Data.SelectedQuestion = null;
        }

        public void SelectRound(Round round)
        {
            Data.SelectedRound = round;
            Data.SelectedTheme = null;
            Data.SelectedQuestion = null;
        }

        public void SelectTheme(Theme theme)
        {
            Data.SelectedTheme = theme;
        }

        public void SelectQuestion(Question question)
        {
            Data.SelectedQuestion = question;
            MetagameEvents.CrafterQuestionSelected.Publish(question);
        }
        
        public void AddPackage()
        {
            string packageArchivePath = PackageFilesSystem.GetPackageArchivePathUsingOpenDialogue();
            
            if (string.IsNullOrEmpty(packageArchivePath))
            {
                Debug.Log("Package is not selected in file browser.");
                return;
            }

            string packagePath = PackageFilesSystem.UnZipArchiveToCrafterFolder(packageArchivePath);
            LoadPackage(packagePath);
        }

        public void SaveSelectedPackage()
        {
            if (Data.SelectedPackage == null)
                return;

            string packageArchivePath = PackageFilesSystem.GetPackageArchivePathUsingSaveDialogue(Data.SelectedPackage.FolderName);

            if (string.IsNullOrEmpty(packageArchivePath))
            {
                Debug.Log($"Package archive path wat not selected. Cancel");
                return;
            }
            
            PackageFilesSystem.SavePackageAsArchive(Data.SelectedPackage, packageArchivePath);
        }

        public void SaveSelectedTheme()
        {
            if (Data.SelectedTheme == null)
                return;
            
            PackageFilesSystem.SaveTheme(Data.SelectedTheme);
        }
        
        public void DeletePackage(Package package)
        {
            PackageFilesSystem.Delete(package);
            Data.Packages.Remove(package);

            if (Data.SelectedPackage == package)
                SelectPackage(null);
        }

        public void CopySelectedThemeToBag()
        {
            if (Data.SelectedTheme == null)
                return;
            
            PackageFilesSystem.SaveTheme(Data.SelectedTheme, PathData.CrafterBagPath);
        }

        public void DeleteTheme(Theme theme)
        {
            PackageTools.DeleteTheme(Data.SelectedPackage, theme);

            if (theme.Questions.Contains(Data.SelectedQuestion))
                SelectQuestion(null);

            SelectTheme(null);
            
            //todo: delete files
            //todo: update package.json
        }

        public void DeleteQuestion(Question question)
        {
            PackageTools.DeleteQuestion(Data.SelectedPackage, question);

            if (Data.SelectedQuestion == question)
                SelectQuestion(null);

            //todo: delete files
            //todo: update package.json
        }

        public void DeleteRound(Round round)
        {
            Data.SelectedPackage.Rounds.Remove(round);
            
            if(Data.SelectedRound == round)
                SelectRound(null);
            
            //todo: delete files
            //todo: update package.json
        }
    }
}