using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class LoadedPackageWidget : MonoBehaviour
    {
        private Package _package;
        
        public Text PackageName;
        public Text Author;
        public Image Background;
        
        public void Bind(Package package, Color color)
        {
            _package = package;
            Background.color = color;
            PackageName.text = $"{package.FolderName}";
            Author.text = $"Автор: {package.Author}";
        }
        
        public void OnDeleteButtonClicked()
        {
            MetagameEvents.LoadedPackageDelete.Publish(_package);
        }

        public void OnCreateButtonClicked()
        {
            MetagameEvents.LoadedPackageCreateGame.Publish(_package);
        }

        public void OnResumeButtonClicked()
        {
            MetagameEvents.LoadedPackageResumeGame.Publish(_package);
        }
    }
}