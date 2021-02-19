using Injection;
using UnityEngine;

namespace Victorina
{
    public class DataChangedHandler
    {
        [Inject] private ViewsSystem ViewsSystem { get; set; }
        [Inject] private PlayerAnswerSystem PlayerAnswerSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        
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
            }

            if (NetworkData.IsMaster)
                return;
            
            //Player
            if (PlayerAnswerSystem.IsTimerRunning && data.TimerState != QuestionTimerState.Running)
            {
                Debug.Log($"Player: StopTimer, {data.TimerState}");
                PlayerAnswerSystem.StopTimer();
                MetagameEvents.QuestionTimerPaused.Publish();
            }

            if (!PlayerAnswerSystem.IsTimerRunning && data.TimerState == QuestionTimerState.Running)
            {
                Debug.Log($"Player: StartTimer, {data.TimerState}, {data.TimerResetSeconds}, {data.TimerLeftSeconds}");
                PlayerAnswerSystem.StartTimer(data.TimerResetSeconds, data.TimerLeftSeconds);
                MetagameEvents.QuestionTimerStarted.Publish();
            }
        }
    }
}