using Injection;
using UnityEngine;

namespace Victorina
{
    public class PackageCrafterSystem
    {
        [Inject] private PackageCrafterData Data { get; set; }
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
        
        public void DeleteSelectedPackage()
        {
            if (Data.SelectedPackage == null)
                return;
            
            PackageFilesSystem.Delete(Data.SelectedPackage);
            Data.Packages.Remove(Data.SelectedPackage);
            SelectPackage(null);
        }

        public void CopySelectedThemeToBag()
        {
            if (Data.SelectedTheme == null)
                return;
            
            PackageFilesSystem.SaveTheme(Data.SelectedTheme, PathData.CrafterBagPath);
        }

        public void DeleteSelectedTheme()
        {
            if (Data.SelectedTheme == null)
                return;

            PackageTools.DeleteTheme(Data.SelectedPackage, Data.SelectedTheme);

            if (Data.SelectedTheme.Questions.Contains(Data.SelectedQuestion))
                SelectQuestion(null);

            SelectTheme(null);
            
            //todo: delete files
            //todo: update package.json
        }

        public void DeleteSelectedQuestion()
        {
            if (Data.SelectedQuestion == null)
                return;
            
            PackageTools.DeleteQuestion(Data.SelectedPackage, Data.SelectedQuestion);
            SelectQuestion(null);
            
            //todo: delete files
            //todo: update package.json
        }
    }
}