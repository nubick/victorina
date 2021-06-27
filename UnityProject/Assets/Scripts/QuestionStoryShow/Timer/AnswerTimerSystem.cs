using System;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class AnswerTimerSystem
    {
        [Inject] private QuestionStripTimer QuestionStripTimer { get; set; }
        [Inject] private AnswerTimerData Data { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private MatchData MatchData { get; set; }

        public void Initialize()
        {
            MetagameEvents.AnswerTimerDataChanged.Subscribe(Refresh);
        }

        private void Refresh()
        {
            if (QuestionStripTimer.IsRunning && Data.State != QuestionTimerState.Running)
            {
                Debug.Log($"StopTimer, {Data.State}, {Data.ResetSeconds}, {Data.LeftSeconds}");
                QuestionStripTimer.Stop();
                MetagameEvents.QuestionTimerPaused.Publish();
            }

            if (!QuestionStripTimer.IsRunning && Data.State == QuestionTimerState.Running)
            {
                Debug.Log($"StartTimer, {Data.State}, {Data.ResetSeconds}, {Data.LeftSeconds}");

                if (NetworkData.IsClient)
                {
                    MatchData.EnableAnswerTime = DateTime.UtcNow;
                    QuestionStripTimer.Reset(Data.ResetSeconds, Data.LeftSeconds);
                    QuestionStripTimer.Start();
                }

                if (NetworkData.IsMaster)
                    QuestionStripTimer.Start();

                MetagameEvents.QuestionTimerStarted.Publish();
            }
        }
    }
}