using System.IO;
using Injection;
using UnityEngine;
using UnityEngine.Networking;

namespace Victorina
{
    public class EncodingFixSystem
    {
        [Inject] private SiqConverter SiqConverter { get; set; }
        
        public void TryFix(string packageName)
        {
            FixURLEncoding(SiqConverter.GetImagesPath(packageName));
            FixURLEncoding(SiqConverter.GetAudioPath(packageName));
            FixURLEncoding(SiqConverter.GetVideoPath(packageName));
        }

        private void FixURLEncoding(string folder)
        {
            if (!Directory.Exists(folder))
                return;
            
            string[] fileNames = Directory.GetFiles(folder);
            foreach (string filePath in fileNames)
            {
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