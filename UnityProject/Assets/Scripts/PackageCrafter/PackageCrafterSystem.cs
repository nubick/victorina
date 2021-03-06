using System;
using System.Linq;
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
            SelectRound(package?.Rounds.FirstOrDefault());
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
        
        public void OpenPackage()
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
                Debug.Log("Package archive path was not selected. Cancel");
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
        
        public void DeleteRound(Round round)
        {
            if (!Data.SelectedPackage.Rounds.Contains(round))
            {
                Debug.LogWarning($"Can't delete round '{round}'. Selected package '{Data.SelectedPackage}' doesn't contain this round");
                return;
            }
            
            Data.SelectedPackage.Rounds.Remove(round);
            
            if(Data.SelectedRound == round)
                SelectRound(null);
            
            PackageFilesSystem.DeleteFiles(round);
            PackageFilesSystem.UpdatePackageJson(Data.SelectedPackage);
        }
        
        public void DeleteTheme(Theme theme)
        {
            if (!Data.SelectedRound.Themes.Contains(theme))
            {
                Debug.LogWarning($"Can't delete theme '{theme}'. Selected round '{Data.SelectedRound}' doesn't contain this theme.");
                return;
            }

            Data.SelectedRound.Themes.Remove(theme);

            if (theme.Questions.Contains(Data.SelectedQuestion))
                SelectQuestion(null);

            SelectTheme(null);

            PackageFilesSystem.DeleteFiles(theme);
            PackageFilesSystem.UpdatePackageJson(Data.SelectedPackage);
        }
        
        public void MoveThemeToBag(Theme theme)
        {
            PackageFilesSystem.SaveTheme(theme, PathData.CrafterBagPath);
            DeleteTheme(theme);
        }
        
        public void DeleteQuestion(Question question)
        {
            PackageTools.DeleteQuestion(Data.SelectedPackage, question);

            if (Data.SelectedQuestion == question)
                SelectQuestion(null);

            PackageFilesSystem.DeleteFiles(question);
            PackageFilesSystem.UpdatePackageJson(Data.SelectedPackage);
        }

        public void ChangeName(Theme theme, string newName)
        {
            bool isValid = !string.IsNullOrWhiteSpace(newName);
            if (isValid)
            {
                Debug.Log($"Change theme name from '{theme.Name}' to '{newName}'");
                theme.Name = newName;
                PackageFilesSystem.UpdatePackageJson(Data.SelectedPackage);
            }
            else
            {
                Debug.Log($"Theme new name '{newName}' is not valid");
            }
        }

        public Theme AddNewTheme()
        {
            Theme newTheme = null;
            if (Data.SelectedRound == null)
            {
                Debug.LogWarning("Can't add new theme. Selected round is null.");
            }
            else
            {
                newTheme = new Theme {Name = "Новая тема"};
                Data.SelectedRound.Themes.Add(newTheme);
                PackageFilesSystem.UpdatePackageJson(Data.SelectedPackage);
            }
            return newTheme;
        }

        public Round AddNewRound(RoundType roundType)
        {
            Round newRound = null;
            if (Data.SelectedPackage == null)
            {
                Debug.LogWarning("Can't add new round. Selected package is null.");
            }
            else
            {
                string roundName = GetRoundDefaultName(roundType, Data.SelectedPackage);
                newRound = new Round {Type = roundType, Name = roundName};
                Data.SelectedPackage.Rounds.Add(newRound);
                PackageFilesSystem.UpdatePackageJson(Data.SelectedPackage);
                SelectRound(newRound);
            }
            return newRound;
        }

        private string GetRoundDefaultName(RoundType roundType, Package package)
        {
            switch (roundType)
            {
                case RoundType.Simple:
                    int nextNumber = package.Rounds.Count + 1;
                    return $"Раунд {nextNumber}";
                case RoundType.Final:
                    return "Финал";
                default:
                    throw new Exception($"Not supported round type: {roundType}");
            }
        }

        public void ChangeName(Round round, string newName)
        {
            bool isValid = !string.IsNullOrWhiteSpace(newName);
            if (isValid)
            {
                Debug.Log($"Change round name from '{round.Name}' to '{newName}'");
                round.Name = newName;
                PackageFilesSystem.UpdatePackageJson(Data.SelectedPackage);
            }
            else
            {
                Debug.Log($"Round new name '{newName}' is not valid");
            }
        }

        public void UpdateSelectedQuestion(QuestionType questionType, int price)
        {
            if (Data.SelectedQuestion.Type != QuestionType.CatInBag && questionType == QuestionType.CatInBag)
                Data.SelectedQuestion.CatInBagInfo = new CatInBagInfo(theme: string.Empty, price, true);

            Data.SelectedQuestion.Type = questionType;
            Data.SelectedQuestion.Price = price;

            PackageFilesSystem.UpdatePackageJson(Data.SelectedPackage);
        }

        public void AddNewPackage(string folderName)
        {
            folderName = folderName.Replace("/", string.Empty);
            Debug.Log($"AddNewPackage: folderName: '{folderName}'");
            
            Package package = new Package();
            package.Path = PackageFilesSystem.SavePackage(package, PathData.CrafterPath, folderName);
            Data.SelectedPackage = package;
            Data.Packages.Add(package);
        }

        public bool CanUseFolderNameForNewPackage(string folderName)
        {
            return PackageFilesSystem.GetCrafterPackagesPaths().Select(System.IO.Path.GetFileName).All(existFolderName => existFolderName != folderName);
        }

        public (int, int) GetBasePriceAndStep(Round round)
        {
            int basePrice = 100;
            int priceStep = 100;
            Theme theme = round.Themes.FirstOrDefault(_ => _.Questions.Count >= 2);
            if (theme != null)
            {
                basePrice = theme.Questions[0].Price;
                priceStep = theme.Questions[1].Price - basePrice;
            }
            return (basePrice, priceStep);
        }

        public void FillRoundPrices(Round round, int basePrice, int priceStep)
        {
            foreach (Theme theme in round.Themes)
            {
                int nextPrice = basePrice;
                foreach (Question question in theme.Questions)
                {
                    question.Price = nextPrice;
                    nextPrice += priceStep;
                }
            }
        }
    }
}