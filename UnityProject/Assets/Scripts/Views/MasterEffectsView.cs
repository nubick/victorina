using Injection;
using UnityEngine;

namespace Victorina
{
    public class MasterEffectsView : ViewBase
    {
        [Inject] private MasterEffectsSystem MasterEffectsSystem { get; set; }
        
        public GameObject EffectsPanel;

        public void Initialize()
        {
            EffectsPanel.SetActive(false);
            Hide();
        }
        
        public void OnShowEffectsButtonClicked()
        {
            EffectsPanel.SetActive(!EffectsPanel.activeSelf);
        }

        public void OnSoundEffectButtonClicked(int number)
        {
            MasterEffectsSystem.PlaySound(number);
        }
    }
}