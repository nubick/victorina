using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class VolumeSettingsWidget : MonoBehaviour
    {
        [Inject] private AppState AppState { get; set; }
        [Inject] private SaveSystem SaveSystem { get; set; }
        
        public Slider VolumeSlider;

        private void OnEnable()
        {
            if (AppState == null)
                return;//Was not injected yet, as all injections work from Start
            
            VolumeSlider.gameObject.SetActive(false);
            VolumeSlider.SetValueWithoutNotify(AppState.Volume.Value);
        }

        public void OnVolumeButtonClicked()
        {
            VolumeSlider.gameObject.SetActive(!VolumeSlider.gameObject.activeSelf);
        }

        public void OnVolumeChanged(float newVolume)
        {
            AppState.Volume.Value = newVolume;
            SaveSystem.Save();
        }
    }
}