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
            if (QuestionTimer.IsRunning && Data.State != QuestionTimerState.Running)
            {
                Debug.Log($"StopTimer, {Data.State}, {Data.ResetSeconds}, {Data.LeftSeconds}");

                if (NetworkData.IsClient)
                    PlayerAnswerSystem.StopTimer();

                if (NetworkData.IsMaster)
                    QuestionTimer.Stop();

                MetagameEvents.QuestionTimerPaused.Publish();
            }

            if (!QuestionTimer.IsRunning && Data.State == QuestionTimerState.Running)
            {
                Debug.Log($"StartTimer, {Data.State}, {Data.ResetSeconds}, {Data.LeftSeconds}");

                if (NetworkData.IsClient)
                    PlayerAnswerSystem.StartTimer(Data.ResetSeconds, Data.LeftSeconds);

                if (NetworkData.IsMaster)
                    QuestionTimer.Start();

                MetagameEvents.QuestionTimerStarted.Publish();
            }
        }
    }
}