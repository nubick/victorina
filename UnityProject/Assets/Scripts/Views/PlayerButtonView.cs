using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerButtonView : ViewBase
    {
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private PlayerAnswerSystem PlayerAnswerSystem { get; set; }

        public Image TimerStrip;
        
        public void OnAnswerButtonClicked()
        {
            PlayerAnswerSystem.SendAnswer();
        }

        public void Update()
        {
            if (IsActive)
            {
                float leftSeconds = QuestionTimer.GetLeftSecondsPercentage();
                TimerStrip.fillAmount = leftSeconds;
                
            }
        }
    }
}