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
            string[] fileNames = Directory.GetFiles(folder);
            foreach (string filePath in fileNames)
            {
                if (filePath.Contains("%"))
                {
                    string unescapedFilePath = UnityWebRequest.UnEscapeURL(filePath);
                    if (unescapedFilePath != filePath)
                    {
                        Debug.Log($"Move file from '{filePath}' to '{unescapedFilePath}'");
                        File.Move(filePath, unescapedFilePath);
                    }
                }
            }
        }
    }
}