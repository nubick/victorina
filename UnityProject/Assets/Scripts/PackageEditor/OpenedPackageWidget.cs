using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class OpenedPackageWidget : MonoBehaviour
    {
        private Package _package;
        
        public Text PackageName;
        public GameObject SelectedState;

        public void Bind(Package package, bool isSelected)
        {
            _package = package;
            PackageName.text = _package.Name;
            SelectedState.SetActive(isSelected);
        }
        
        public void OnSelectButtonClicked()
        {
            MetagameEvents.EditorPackageClicked.Publish(_package);
        }
    }
}