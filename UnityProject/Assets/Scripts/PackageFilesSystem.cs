using System;
using System.Collections.Generic;
using Injection;
using System.IO;
using System.IO.Compression;
using SFB;
using UnityEngine;

namespace Victorina
{
    public class PackageFilesSystem
    {
        private const string VumFilterName = "Vumka Files";
        private const string VumFilterExtension = "vum";
        private const string SiqFilterName = "SIQ Files";
        private const string SiqFilterExtension = "siq";
        
        [Inject] private PathData PathData { get; set; }
        [Inject] private SiqConverter SiqConverter { get; set; }
        [Inject] private PackageJsonConverter PackageJsonConverter { get; set; }

        #region Archive file
        
        public string GetPackageArchivePathUsingOpenDialogue()
        {
            ExtensionFilter[] extensions =
            {
                new ExtensionFilter(VumFilterName, VumFilterExtension),
                new ExtensionFilter(SiqFilterName, SiqFilterExtension)
            };
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            return paths[0];
        }

        public string GetPackageArchivePathUsingSaveDialogue(string packageName)
        {
            ExtensionFilter[] extensions = {new ExtensionFilter(VumFilterName, VumFilterExtension)};
            string dialogueTitle = "Save Vumka Package";
            string rootDirectory = "";
            string defaultName = packageName;
            return StandaloneFileBrowser.SaveFilePanel(dialogueTitle, rootDirectory, defaultName, extensions);
        }

        public string UnZipArchiveToPlayFolder(string packageArchivePath)
        {
            return UnZipArchiveToFolder(packageArchivePath, PathData.PackagesPath);
        }

        public string UnZipArchiveToCrafterFolder(string packageArchivePath)
        {
            return UnZipArchiveToFolder(packageArchivePath, PathData.CrafterPath);
        }

        private string UnZipArchiveToFolder(string packageArchivePath, string destinationFolderPath)
        {
            string archiveName = Path.GetFileNameWithoutExtension(packageArchivePath);
            string destinationPath = $"{destinationFolderPath}/{archiveName}";
            
            UnZip(packageArchivePath, destinationPath);
            
            if (SiqConverter.IsValid(destinationPath))
            {
                ConvertSiqPackage(destinationPath, destinationFolderPath);
            }
            
            return destinationPath;
        }

        private void EnsureTempFolderIsEmpty()
        {
            if (Directory.Exists(PathData.TempArchivePath))
            {
                Debug.LogWarning($"Temp folder is not empty. Clear temp folder: {PathData.TempArchivePath}");
                Directory.Delete(PathData.TempArchivePath, recursive: true);
            }
        }
        
        private void ConvertSiqPackage(string siqPackagePath, string parentFolder)
        {
            Debug.Log($"Siq archive detected. Move to temp folder: {PathData.TempArchivePath}");
            EnsureTempFolderIsEmpty();
            Directory.Move(siqPackagePath, PathData.TempArchivePath);
            Package package = SiqConverter.Convert(PathData.TempArchivePath);
            string folderName = Path.GetFileName(siqPackagePath);
            SavePackage(package, parentFolder, folderName);
            EnsureTempFolderIsEmpty();
        }

        public void SavePackageAsArchive(Package package, string packageArchivePath)
        {
            EnsureTempFolderIsEmpty();
            SavePackage(package, PathData.TempArchivePath);
            string sourceDirectoryName = $"{PathData.TempArchivePath}/{package.FolderName}";
            ZipFile.CreateFromDirectory(sourceDirectoryName, packageArchivePath);
            EnsureTempFolderIsEmpty();
            Debug.Log($"Package is saved to archive: {packageArchivePath}");
        }
        
        private void UnZip(string packageArchivePath, string destinationPath)
        {
            Debug.Log($"UnZip from '{packageArchivePath}' to '{destinationPath}'");
            
            bool exists = File.Exists(packageArchivePath);
            if (!exists)
                throw new Exception($"File doesn't exist: {packageArchivePath}");
            
            ZipFile.ExtractToDirectory(packageArchivePath, destinationPath);
        }
        
        #endregion
        
        public string[] GetCrafterPackagesPaths()
        {
            return Directory.GetDirectories(PathData.CrafterPath);
        }

        public Package LoadPackage(string packagePath)
        {
            string jsonPath = $"{packagePath}/package.json";
            string json = File.ReadAllText(jsonPath);
            Package package = PackageJsonConverter.ReadPackage(json);
            package.Path = packagePath;
            FillFilePaths(package);
            return package;
        }

        private void FillFilePaths(Package package)
        {
            foreach (StoryDot storyDot in PackageTools.GetAllStories(package))
            {
                if (storyDot is FileStoryDot fileStoryDot)
                    fileStoryDot.Path = $"{package.Path}/{fileStoryDot.FileName}";
            }
        }

        public void Delete(string packagePath)
        {
            Debug.Log($"Delete package with path: {packagePath}");
            if (Directory.Exists(packagePath))
            {
                Directory.Delete(packagePath, true);
                Debug.Log("Package deletion is done.");
            }
            else
            {
                Debug.Log($"Package directory doesn't exist: {packagePath}");
            }
        }

        public void Delete(Package package)
        {
            Delete(package.Path);
        }

        public void SaveTheme(Theme theme, string rootFolderPath)
        {
            PackageJsonConverter jsonConverter = new PackageJsonConverter();
            string json = jsonConverter.ToJson(theme);
            
            string themePath = $"{rootFolderPath}/{theme.Name}";
            Directory.CreateDirectory(themePath);
            File.WriteAllText($"{themePath}/theme.json", json);
            
            CopyFiles(theme, themePath);
            Debug.Log($"Theme is saved: {theme.Name}");
        }
        
        public void SaveTheme(Theme theme)
        {
            SaveTheme(theme, PathData.CrafterPath);
        }

        public void SavePackage(Package package, string parentFolderPath, string packageFolderName = null)
        {
            string json = PackageJsonConverter.ToJson(package);

            string folderName = packageFolderName ?? package.FolderName;
            string packageFolderPath = $"{parentFolderPath}/{folderName}";

            if (Directory.Exists(packageFolderPath))
                throw new Exception($"Can't save package. Folder exists: {packageFolderPath}");

            Debug.Log($"Create directory: {packageFolderPath}");
            Directory.CreateDirectory(packageFolderPath);
            string jsonPath = $"{packageFolderPath}/package.json";
            File.WriteAllText(jsonPath, json);
            Debug.Log($"Package json is saved: {jsonPath}");
            
            CopyFiles(package, packageFolderPath);
            
            Debug.Log($"Package is saved: {packageFolderPath}");
        }
        
        private void CopyFiles(Package package, string packageFolderPath)
        {
            CopyFiles(PackageTools.GetAllQuestions(package), packageFolderPath);
        }
        
        private void CopyFiles(Theme theme, string themePath)
        {
            CopyFiles(theme.Questions, themePath);
        }

        private void CopyFiles(IEnumerable<Question> questions, string packageFolderPath)
        {
            foreach (Question question in questions)
            {
                CopyStoryFiles(question.QuestionStory, packageFolderPath);
                CopyStoryFiles(question.AnswerStory, packageFolderPath);
            }
        }
        
        private void CopyStoryFiles(List<StoryDot> story, string packageFolderPath)
        {
            foreach (StoryDot storyDot in story)
            {
                if (storyDot is FileStoryDot fileStoryDot)
                {
                    string newFilePath = $"{packageFolderPath}/{fileStoryDot.FileName}";

                    if (!File.Exists(fileStoryDot.Path))
                        throw new Exception($"Can't copy file. File is missed: {fileStoryDot.Path}");

                    if (File.Exists(newFilePath))
                        throw new Exception($"Can't copy file. File exists by path: '{newFilePath}'");
                    
                    Debug.Log($"File: {fileStoryDot.FileName}");
                    File.Copy(fileStoryDot.Path, newFilePath);
                }
            }
        }
    }
}