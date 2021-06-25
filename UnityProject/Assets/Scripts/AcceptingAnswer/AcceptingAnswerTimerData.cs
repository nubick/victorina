namespace Victorina
{
    public class AcceptingAnswerTimerData
    {
        public bool IsRunning { get; set; }
        public float MaxSeconds { get; set; }
        public float LeftSeconds { get; set; }
        
        //Master Only
        public float LastTimeSend { get; set; }
    }
}