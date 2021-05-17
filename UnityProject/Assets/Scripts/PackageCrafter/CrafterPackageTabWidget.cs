using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterPackageTabWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Package _package;
        
        public Text PackageName;
        public GameObject SelectedState;
        public GameObject DeleteButton;

        public void Bind(Package package, bool isSelected)
        {
            _package = package;
            PackageName.text = _package.FolderName;
            SelectedState.SetActive(isSelected);
            DeleteButton.SetActive(false);
        }
        
        public void OnSelectButtonClicked()
        {
            MetagameEvents.CrafterPackageClicked.Publish(_package);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            DeleteButton.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DeleteButton.SetActive(false);
        }

        public void OnDeleteButtonClicked()
        {
            MetagameEvents.CrafterPackageDeleteButtonClicked.Publish(_package);
        }
    }
}