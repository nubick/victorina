using Injection;
using UnityEngine;

namespace Victorina
{
    public class TimerRunOutDetectSystem
    {
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private QuestionStripTimer QuestionStripTimer { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }

        public void OnUpdate()
        {
            return;
            
            if (!NetworkData.IsMaster)
                return;
            
            if (AnswerTimerData.State == QuestionTimerState.Running)
            {
                float leftSecondsPercentage = QuestionStripTimer.GetLeftSecondsPercentage();
                bool isRunOutOfTime = Mathf.Approximately(leftSecondsPercentage, 0f);
                if (isRunOutOfTime)
                {
                    AnswerTimerData.State = QuestionTimerState.RunOut;
                    MetagameEvents.TimerRunOut.Publish();
                }
            }
        }
    }
}