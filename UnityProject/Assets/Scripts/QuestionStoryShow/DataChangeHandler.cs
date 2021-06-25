using Injection;
using UnityEngine;

namespace Victorina
{
    public class DataChangeHandler
    {
        [Inject] private ViewsSystem ViewsSystem { get; set; }
        
        
        

        public void HandleMasterIntention()
        {
            //Common
            //switch (data.MasterIntention)
            {
                //todo: finish refactoring
                // case MasterIntention.RestartMedia:
                //     MetagameEvents.MediaRestarted.Publish();
                //     break;
            }

            //Debug.Log($"Timer LOGS, state: {data.TimerState}, timer.IsRunning: {QuestionTimer.IsRunning}, {data.TimerResetSeconds}, {data.TimerLeftSeconds}");
            
            
            
            
        }
    }
}