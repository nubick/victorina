using Injection;
using UnityEngine;

namespace Victorina
{
    public class PackageCrafterSystem
    {
        [Inject] private PackageEditorData Data { get; set; }
        [Inject] private PackageFilesSystem PackageFilesSystem { get; set; }
        [Inject] private SiqPackageOpenSystem SiqPackageOpenSystem { get; set; }
        [Inject] private PackageEditorSaveSystem SaveSystem { get; set; }
        
        public void LoadPackages()
        {
            Data.Packages.Clear();
            foreach (string packagePath in PackageFilesSystem.GetCrafterPackagesPaths())
            {
                Package package = PackageFilesSystem.LoadPackage(packagePath);
                Data.Packages.Add(package);
            }
            SelectPackage(null);
        }
        
        public void SelectPackage(Package package)
        {
            Data.SelectedPackage = package;
            Data.SelectedRound = null;
            Data.SelectedTheme = null;
        }

        public void SelectRound(Round round)
        {
            Data.SelectedRound = round;
            Data.SelectedTheme = null;
        }

        public void SelectTheme(Theme theme)
        {
            Data.SelectedTheme = theme;
        }
        
        public void AddPackage()
        {
            string packagePath = SiqPackageOpenSystem.GetPathUsingOpenDialogue();

            if (string.IsNullOrEmpty(packagePath))
            {
                Debug.Log("Package is not selected in file browser.");
                return;
            }

            SiqPackageOpenSystem.UnZipPackageToEditorFolder(packagePath);
            LoadPackages();
        }

        public void SaveSelectedPackage()
        {
            if (Data.SelectedPackage == null)
                return;

            SaveSystem.SavePackage(Data.SelectedPackage);
        }

        public void SaveSelectedTheme()
        {
            if (Data.SelectedTheme == null)
                return;
            
            SaveSystem.SaveTheme(Data.SelectedPackage, Data.SelectedTheme);
        }
        
        public void DeleteSelectedPackage()
        {
            if (Data.SelectedPackage == null)
                return;
            
            PackageFilesSystem.Delete(Data.SelectedPackage);
            Data.Packages.Remove(Data.SelectedPackage);
            SelectPackage(null);
        }
    }
}