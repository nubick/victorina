using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PackageEditorSaveSystem
    {
        [Inject] private PathData PathData { get; set; }
        [Inject] private PackageJsonConverter PackageJsonConverter { get; set; }
        
        public void SavePackage(Package package)
        {
            string json = PackageJsonConverter.ToJson(package);

            string folderName = $"package_{package.Name}";
            string packageFolderPath = $"{PathData.PackageEditorPath}/{folderName}";
            Directory.CreateDirectory(packageFolderPath);
            File.WriteAllText($"{packageFolderPath}/package.json", json);

            Debug.Log($"Package json is saved");
            
            CopyFiles(package, packageFolderPath);
            
            Debug.Log($"Package is saved: {package.Name}");
        }
        
        public void SaveTheme(Package package, Theme theme)
        {
            PackageJsonConverter jsonConverter = new PackageJsonConverter();

            string json = jsonConverter.ToJson(theme);

            string folderName = theme.Name;
            string themePath = $"{PathData.PackageEditorPath}/{folderName}";
            Directory.CreateDirectory(themePath);
            File.WriteAllText($"{themePath}/theme.json", json);
            
            CopyFiles(theme, themePath);

            Debug.Log($"Theme is saved: {theme.Name}");
        }

        private void CopyFiles(Package package, string packageFolderPath)
        {
            var questions = package.Rounds.SelectMany(round => round.Themes.SelectMany(theme => theme.Questions));
            foreach (Question question in questions)
            {
                CopyStoryFiles(question.QuestionStory, packageFolderPath);
                CopyStoryFiles(question.AnswerStory, packageFolderPath);
            }
        }
        
        private void CopyFiles(Theme theme, string themePath)
        {
            foreach (Question question in theme.Questions)
            {
                CopyStoryFiles(question.QuestionStory, themePath);
                CopyStoryFiles(question.AnswerStory, themePath);
            }
        }

        private void CopyStoryFiles(List<StoryDot> story, string path)
        {
            foreach (StoryDot storyDot in story)
            {
                if (storyDot is FileStoryDot fileStoryDot)
                {
                    string fileName = Path.GetFileName(fileStoryDot.SiqPath);
                    string filePath = $"{path}/{fileName}";

                    if (File.Exists(fileStoryDot.SiqPath))
                        throw new Exception($"Can't copy file. File exists by path: '{fileStoryDot.SiqPath}'");
                    
                    File.Copy(fileStoryDot.SiqPath, filePath);
                    Debug.Log($"File was copied: {fileName}");
                }
            }
        }
    }
}