using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class PackageEditorRoundTabWidget : MonoBehaviour
    {
        private Round _round;
        
        public Text RoundName;
        public GameObject SelectedState;
        
        public void Bind(Round round, bool isSelected)
        {
            _round = round;
            RoundName.text = _round.Name;
            SelectedState.SetActive(isSelected);
        }

        public void OnClicked()
        {
            MetagameEvents.CrafterRoundClicked.Publish(_round);
        }
    }
}