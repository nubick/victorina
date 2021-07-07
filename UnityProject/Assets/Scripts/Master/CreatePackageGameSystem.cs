using System.IO;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class CreatePackageGameSystem
    {
        [Inject] private CreatePackageGameData Data { get; set; }
        [Inject] private PackageFilesSystem PackageFilesSystem { get; set; }
        [Inject] private PathData PathData { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private PlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayJournalSystem PlayJournalSystem { get; set; }

        public void RefreshPackages()
        {
            Data.Packages.Clear();
            string[] fullPaths = Directory.GetDirectories(PathData.PackagesPath);
            foreach (string packageFullPath in fullPaths)
            {
                Package package = PackageFilesSystem.LoadPackage(packageFullPath);
                Data.Packages.Add(package);
                package.HasJournal = PlayJournalSystem.HasJournal(package);
            }
            Debug.Log($"Loaded packages: {Data.Packages.Count}");
        }

        public void LoadPackage()
        {
            string packageArchivePath = PackageFilesSystem.GetPackageArchivePathUsingOpenDialogue();

            if (string.IsNullOrWhiteSpace(packageArchivePath))
            {
                Debug.Log("Package archive is not selected in file browser.");
                return;
            }
            
            PackageFilesSystem.UnZipArchiveToPlayFolder(packageArchivePath);
            
            string packageName = Path.GetFileName(packageArchivePath);
            AnalyticsEvents.LoadPackage.Publish(packageName);
            
            RefreshPackages();
        }

        private void StartPackageGame(Package package)
        {
            PackageSystem.StartPackageGame(package);
            MatchSystem.StartMatch();
            ServerService.StartServer();
            PlayStateSystem.StartPackageOnLobby();
        }
        
        public void CreatePackageGame(Package package)
        {
            StartPackageGame(package);
            PlayJournalSystem.Clear();
            AnalyticsEvents.StartPackageGame.Publish(package.FolderName);
        }

        public void ResumePackageGame(Package package)
        {
            StartPackageGame(package);
            PlayJournalSystem.Play(package);
            AnalyticsEvents.ResumePackageGame.Publish(package.FolderName);
        }
        
        public void Delete(Package package)
        {
            PackageFilesSystem.Delete(package.Path);
            RefreshPackages();
        }
    }
}