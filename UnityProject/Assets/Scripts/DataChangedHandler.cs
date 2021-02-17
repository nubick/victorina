using Injection;

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
            
            if(PlayerAnswerSystem.IsTimerRunning && data.TimerState != QuestionTimerState.Running)
                PlayerAnswerSystem.StopTimer();

            if (!PlayerAnswerSystem.IsTimerRunning && data.TimerState == QuestionTimerState.Running)
                PlayerAnswerSystem.StartTimer(data.TimerResetSeconds, data.TimerLeftSeconds);
        }
    }
}