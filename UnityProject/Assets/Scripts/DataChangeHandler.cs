using Injection;
using UnityEngine;

namespace Victorina
{
    public class DataChangeHandler
    {
        [Inject] private ViewsSystem ViewsSystem { get; set; }
        [Inject] private PlayerAnswerSystem PlayerAnswerSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private QuestionTimer QuestionTimer { get; set; }

        public void HandleMasterIntention(QuestionAnswerData data)
        {
            //Common
            switch (data.MasterIntention)
            {
                case MasterIntention.StartAnswering:
                    ViewsSystem.StartAnswering();
                    break;
                case MasterIntention.ShowStoryDot:
                    ViewsSystem.UpdateStoryDot(data);
                    break;
                case MasterIntention.ShowAnswer:
                    ViewsSystem.UpdateStoryDot(data);
                    break;
                case MasterIntention.RestartMedia:
                    MetagameEvents.MediaRestarted.Publish();
                    break;
            }

            Debug.Log($"Timer LOGS, state: {data.TimerState}, timer.IsRunning: {QuestionTimer.IsRunning}, {data.TimerResetSeconds}, {data.TimerLeftSeconds}");
            
            if (QuestionTimer.IsRunning && data.TimerState != QuestionTimerState.Running)
            {
                Debug.Log($"StopTimer, {data.TimerState}, {data.TimerResetSeconds}, {data.TimerLeftSeconds}");
                if (NetworkData.IsClient)
                    PlayerAnswerSystem.StopTimer();
                else
                    QuestionTimer.Stop();
                MetagameEvents.QuestionTimerPaused.Publish();
            }

            if (!QuestionTimer.IsRunning && data.TimerState == QuestionTimerState.Running)
            {
                Debug.Log($"StartTimer, {data.TimerState}, {data.TimerResetSeconds}, {data.TimerLeftSeconds}");
                if (NetworkData.IsClient)
                    PlayerAnswerSystem.StartTimer(data.TimerResetSeconds, data.TimerLeftSeconds);
                else
                    QuestionTimer.Start();
                MetagameEvents.QuestionTimerStarted.Publish();
            }
        }
    }
}