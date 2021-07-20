using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class MatchSettingsView : ViewBase, IDataDependOnlyView
    {
        [Inject] private MatchSettingsData Data { get; set; }
        
        public Toggle LimitAnsweringSecondsToggle;
        public InputField MaxAnsweringSecondsInputField;
        
        protected override void OnShown()
        {
            LimitAnsweringSecondsToggle.isOn = Data.IsLimitAnsweringSeconds;
            MaxAnsweringSecondsInputField.text = Data.MaxAnsweringSeconds.ToString("0.0");
        }
        
        public void OnSaveButtonClicked()
        {
            Data.IsLimitAnsweringSeconds = LimitAnsweringSecondsToggle.isOn;
            if (float.TryParse(MaxAnsweringSecondsInputField.text, out float seconds))
            {
                seconds = Mathf.Max(1f, seconds);
                Data.MaxAnsweringSeconds = seconds;
            }
            Hide();
        }
        
        public void OnCancelButtonClicked()
        {
            Hide();
        }
    }
}