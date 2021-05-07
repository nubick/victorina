using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class OpenedPackageWidget : MonoBehaviour
    {
        public Text PackageName;
        public GameObject SelectedState;

        public void Bind(string packageName, bool isSelected)
        {
            PackageName.text = packageName;
            SelectedState.SetActive(isSelected);
        }
        
        public void OnSelectButtonClicked()
        {
            MetagameEvents.EditorPackageClicked.Publish(PackageName.text);
        }
        
        public void OnDeleteButtonClicked()
        {
            
        }
    }
}