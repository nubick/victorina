using Injection;
using UnityEngine;

namespace Victorina
{
    public class AnswerTimerSystem
    {
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private AnswerTimerData Data { get; set; }
        [Inject] private PlayerAnswerSystem PlayerAnswerSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }

        public void Initialize()
        {
            MetagameEvents.AnswerTimerDataChanged.Subscribe(Refresh);
        }

        private void Refresh()
        {
            if (QuestionTimer.IsRunning && Data.TimerState != QuestionTimerState.Running)
            {
                Debug.Log($"StopTimer, {Data.TimerState}, {Data.TimerResetSeconds}, {Data.TimerLeftSeconds}");
                if (NetworkData.IsClient)
                    PlayerAnswerSystem.StopTimer();
                else
                    QuestionTimer.Stop();
                MetagameEvents.QuestionTimerPaused.Publish();
            }

            if (!QuestionTimer.IsRunning && Data.TimerState == QuestionTimerState.Running)
            {
                Debug.Log($"StartTimer, {Data.TimerState}, {Data.TimerResetSeconds}, {Data.TimerLeftSeconds}");
                if (NetworkData.IsClient)
                    PlayerAnswerSystem.StartTimer(Data.TimerResetSeconds, Data.TimerLeftSeconds);
                else
                    QuestionTimer.Start();
                MetagameEvents.QuestionTimerStarted.Publish();
            }
        }
    }
}