using Injection;
using UnityEngine;

namespace Victorina
{
    public class TimerRunOutDetectSystem
    {
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }

        public void OnUpdate()
        {
            if (!NetworkData.IsMaster)
                return;
            
            if (AnswerTimerData.TimerState == QuestionTimerState.Running)
            {
                float leftSecondsPercentage = QuestionTimer.GetLeftSecondsPercentage();
                bool isRunOutOfTime = Mathf.Approximately(leftSecondsPercentage, 0f);
                if (isRunOutOfTime)
                {
                    AnswerTimerData.TimerState = QuestionTimerState.RunOut;
                    MetagameEvents.TimerRunOut.Publish();
                }
            }
        }
    }
}