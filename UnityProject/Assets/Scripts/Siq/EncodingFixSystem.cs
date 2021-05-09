using System.IO;
using Injection;
using UnityEngine;
using UnityEngine.Networking;

namespace Victorina
{
    public class EncodingFixSystem
    {
        [Inject] private SiqConverter SiqConverter { get; set; }
        
        public void TryFix(string packagePath)
        {
            FixURLEncoding(SiqConverter.GetImagesPath(packagePath));
            FixURLEncoding(SiqConverter.GetAudioPath(packagePath));
            FixURLEncoding(SiqConverter.GetVideoPath(packagePath));
        }

        private void FixURLEncoding(string folder)
        {
            if (!Directory.Exists(folder))
                return;
            
            string[] fileNames = Directory.GetFiles(folder);
            foreach (string filePath in fileNames)
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"SKIP: Can't find file by path: {filePath}");
                    continue;
                }

                if (filePath.Contains("%"))
                {
                    string unescapedFilePath = UnityWebRequest.UnEscapeURL(filePath);
                    if (unescapedFilePath != filePath)
                    {
                        Debug.Log($"Unescape file name '{filePath}' to '{unescapedFilePath}'");
                        if (File.Exists(unescapedFilePath))
                            Debug.LogWarning($"Can't move, file with such name exists: {unescapedFilePath}");
                        else
                            File.Move(filePath, unescapedFilePath);
                    }
                }
            }
        }
    }
}