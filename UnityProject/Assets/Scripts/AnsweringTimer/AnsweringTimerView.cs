using UnityEngine.UI;

namespace Victorina
{
    public class AnsweringTimerView : ViewBase
    {
        public Image CircleIndicator;
        public Text LeftSeconds;

        public void RefreshUI(AnsweringTimerData data)
        {
            LeftSeconds.text = $"{data.LeftSeconds:0.0}";
            CircleIndicator.fillAmount = data.LeftSeconds / data.MaxSeconds;
        }
    }
}