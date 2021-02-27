using Injection;
using UnityEngine;

namespace Victorina
{
    public class TimerRunOutDetectSystem
    {
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private QuestionTimer QuestionTimer { get; set; }

        public void OnUpdate()
        {
            if (!NetworkData.IsMaster)
                return;
            
            if (QuestionAnswerData.TimerState == QuestionTimerState.Running)
            {
                float leftSecondsPercentage = QuestionTimer.GetLeftSecondsPercentage();
                bool isRunOutOfTime = Mathf.Approximately(leftSecondsPercentage, 0f);
                if (isRunOutOfTime)
                {
                    QuestionAnswerData.TimerState = QuestionTimerState.RunOut;
                    MetagameEvents.TimerRunOut.Publish();
                }
            }
        }
    }
}