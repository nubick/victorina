using System;
using System.Collections.Generic;
using Injection;
using System.IO;
using System.IO.Compression;
using SFB;
using UnityEngine;
using UnityEngine.Networking;

namespace Victorina
{
    public class PackageFilesSystem
    {
        private const string VumFilterName = "Vumka Files";
        private const string VumFilterExtension = "vum";
        private const string SiqFilterName = "SIQ Files";
        private const string SiqFilterExtension = "siq";
        private const string PackageJsonFileName = "package.json";
        private const string ThemeJsonFileName = "theme.json";
        
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
                Debug.Log($"Temp folder is not empty. Clear temp folder: {PathData.TempArchivePath}");
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
            string archiveName = Path.GetFileName(packageArchivePath);
            AnalyticsEvents.SavePackageAsArchive.Publish(archiveName);
        }
        
        private void UnZip(string packageArchivePath, string destinationPath)
        {
            Debug.Log($"UnZip from '{packageArchivePath}' to '{destinationPath}'");
            
            bool exists = File.Exists(packageArchivePath);
            if (!exists)
                throw new Exception($"File doesn't exist: {packageArchivePath}");

            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);
            
            ZipArchive zipArchive = ZipFile.OpenRead(packageArchivePath);
            foreach (ZipArchiveEntry entry in zipArchive.Entries)
            {
                string fullName = entry.FullName;
                if (fullName.Contains("%"))
                    fullName = UnityWebRequest.UnEscapeURL(entry.FullName);
                
                string filePath = $"{destinationPath}/{fullName}";
                string fileDirectory = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);

                entry.ExtractToFile(filePath);
            }
        }
        
        #endregion
        
        public string[] GetCrafterPackagesPaths()
        {
            return Directory.GetDirectories(PathData.CrafterPath);
        }

        public Package LoadPackage(string packageFolderPath)
        {
            string jsonPath = $"{packageFolderPath}/{PackageJsonFileName}";
            string json = File.ReadAllText(jsonPath);
            Package package = PackageJsonConverter.ReadPackage(json);
            package.Path = packageFolderPath;
            FillFilePaths(PackageTools.GetAllFileStoryDots(package), package.Path);
            ValidateFilesPresence(PackageTools.GetAllFileStoryDots(package));
            return package;
        }

        public Theme LoadTheme(string themeFolderPath)
        {
            string jsonFilePath = $"{themeFolderPath}/{ThemeJsonFileName}";
            string json = File.ReadAllText(jsonFilePath);
            Theme theme = PackageJsonConverter.ReadTheme(json);
            FillFilePaths(theme, themeFolderPath);
            ValidateFilesPresence(PackageTools.GetAllFileStoryDots(theme));
            return theme;
        }

        public void FillFilePaths(Theme theme, string folderPath)
        {
            FillFilePaths(PackageTools.GetAllFileStoryDots(theme), folderPath);
        }
        
        private void FillFilePaths(IEnumerable<FileStoryDot> fileStoryDots, string folderPath)
        {
            foreach(FileStoryDot fileStoryDot in fileStoryDots)
                fileStoryDot.Path = $"{folderPath}/{fileStoryDot.FileName}";
        }
        
        private void ValidateFilesPresence(IEnumerable<FileStoryDot> fileStoryDots)
        {
            foreach (FileStoryDot fileStoryDot in fileStoryDots)
            {
                if (!File.Exists(fileStoryDot.Path))
                    Debug.LogWarning($"File doesn't exist by path: '{fileStoryDot.Path}'");
            }
        }

        public void UpdatePackageJson(Package package)
        {
            string jsonPath = $"{package.Path}/{PackageJsonFileName}";
            if (File.Exists(jsonPath))
            {
                string packageJson = PackageJsonConverter.ToJson(package);
                File.WriteAllText(jsonPath, packageJson);
            }
            else
            {
                Debug.LogWarning($"Can't update '{PackageJsonFileName}' file. It doesn't exist by path: {jsonPath}");
            }
        }
        
        public void Delete(string folderPath)
        {
            Debug.Log($"Delete folder with path: {folderPath}");
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
                Debug.Log("Folder deletion is done.");
            }
            else
            {
                Debug.Log($"Folder doesn't exist: {folderPath}");
            }
        }

        public void Delete(Package package)
        {
            Delete(package.Path);
        }

        public void SaveTheme(Theme theme, string rootFolderPath)
        {
            string themeFolderPath = $"{rootFolderPath}/{theme.Name}";

            if (Directory.Exists(themeFolderPath))
            {
                Debug.LogWarning($"Can't save theme, folder exists: {themeFolderPath}");
                return;
            }
            
            Directory.CreateDirectory(themeFolderPath);
            CopyFiles(theme, themeFolderPath);
            
            string json = PackageJsonConverter.ToJson(theme);
            File.WriteAllText($"{themeFolderPath}/{ThemeJsonFileName}", json);
            
            Debug.Log($"Theme is saved: {theme.Name}");
        }
        
        public void SaveTheme(Theme theme)
        {
            SaveTheme(theme, PathData.CrafterPath);
        }

        public void DeleteFiles(Round round)
        {
            foreach(Theme theme in round.Themes)
                DeleteFiles(theme);
        }

        private void DeleteFiles(IEnumerable<FileStoryDot> fileStoryDots)
        {
            foreach (FileStoryDot fileStoryDot in fileStoryDots)
            {
                Debug.Log($"Delete file: {fileStoryDot.Path}");
                if (File.Exists(fileStoryDot.Path))
                    File.Delete(fileStoryDot.Path);
                else
                    Debug.LogWarning($"Can't delete file. It doesn't exist: {fileStoryDot.Path}");
            }
        }

        public void DeleteFiles(Theme theme)
        {
            DeleteFiles(PackageTools.GetAllFileStoryDots(theme));
        }

        public void DeleteFiles(Question question)
        {
            DeleteFiles(PackageTools.GetAllFileStoryDots(question));
        }

        public void DeleteBagTheme(Theme theme)
        {
            string themeFolderPath = $"{PathData.CrafterBagPath}/{theme.Name}";
            Delete(themeFolderPath);
        }
        
        public string SavePackage(Package package, string parentFolderPath, string packageFolderName = null)
        {
            string json = PackageJsonConverter.ToJson(package);

            string folderName = packageFolderName ?? package.FolderName;
            string packageFolderPath = $"{parentFolderPath}/{folderName}";

            if (Directory.Exists(packageFolderPath))
                throw new Exception($"Can't save package. Folder exists: {packageFolderPath}");

            Debug.Log($"Create directory: {packageFolderPath}");
            Directory.CreateDirectory(packageFolderPath);
            string jsonPath = $"{packageFolderPath}/{PackageJsonFileName}";
            File.WriteAllText(jsonPath, json);
            Debug.Log($"Package json is saved: {jsonPath}");
            
            CopyFiles(package, packageFolderPath);
            
            Debug.Log($"Package is saved: {packageFolderPath}");
            return packageFolderPath;
        }
        
        private void CopyFiles(Package package, string packageFolderPath)
        {
            CopyStoryFiles(PackageTools.GetAllFileStoryDots(package), packageFolderPath);
        }
        
        private void CopyFiles(Theme theme, string themeFolderPath)
        {
            CopyStoryFiles(PackageTools.GetAllFileStoryDots(theme), themeFolderPath);
        }
        
        public void CopyFiles(Theme theme, Package package)
        {
            CopyStoryFiles(PackageTools.GetAllFileStoryDots(theme), package.Path);
        }
        
        private void CopyStoryFiles(IEnumerable<FileStoryDot> fileStoryDots, string destinationFolderPath)
        {
            foreach (FileStoryDot fileStoryDot in fileStoryDots)
            {
                string newFilePath = $"{destinationFolderPath}/{fileStoryDot.FileName}";

                if (!File.Exists(fileStoryDot.Path))
                {
                    Debug.LogWarning($"Can't copy file. File is missed: {fileStoryDot.Path}");
                    continue;
                }

                if (File.Exists(newFilePath))
                {
                    if (IsSameFiles(fileStoryDot.Path, newFilePath))
                    {
                        Debug.Log($"Don't need to copy. The same file exists by path: {newFilePath}");
                    }
                    else
                    {
                        string emptyFileName = GetEmptyFileName(fileStoryDot.FileName, destinationFolderPath);
                        Debug.Log($"Different files with the same file name '{fileStoryDot.FileName}', take new file name: {emptyFileName}");
                        string emptyFilePath = $"{destinationFolderPath}/{emptyFileName}";
                        File.Copy(fileStoryDot.Path, emptyFilePath);
                        fileStoryDot.FileName = emptyFileName;
                    }
                }
                else
                {
                    Debug.Log($"Copy file '{fileStoryDot.FileName}' to '{newFilePath}'");
                    File.Copy(fileStoryDot.Path, newFilePath);
                }
            }
        }

        private bool IsSameFiles(string path1, string path2)
        {
            if (path1 == path2)
                return true;

            if (!File.Exists(path1))
                throw new Exception($"Can't compare two files. File doesn't exist: {path1}");

            if (!File.Exists(path2))
                throw new Exception($"Can't compare two files. File doesn't exist: {path2}");
            
            byte[] bytes1 = File.ReadAllBytes(path1);
            byte[] bytes2 = File.ReadAllBytes(path2);

            if (bytes1.Length != bytes2.Length)
                return false;

            for (int i = 0; i < bytes1.Length; i++)
            {
                if (bytes1[i] != bytes2[i])
                    return false;
            }

            return true;
        }

        private string GetEmptyFileName(string fileName, string folderPath)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            for (int number = 2;; number++)
            {
                string newFileName = $"{fileNameWithoutExtension} ({number}){extension}";
                string filePath = $"{folderPath}/{newFileName}";
                if (!File.Exists(filePath))
                    return newFileName;
            }
        }
    }
}