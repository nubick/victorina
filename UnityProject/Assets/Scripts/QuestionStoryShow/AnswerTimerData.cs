namespace Victorina
{
    public class AnswerTimerData : SyncData
    {
        private QuestionTimerState _timerState;
        public QuestionTimerState TimerState
        {
            get => _timerState;
            set
            {
                _timerState = value; 
                MarkAsChanged();
            }
        }

        private float _timerResetSeconds;
        public float TimerResetSeconds
        {
            get => _timerResetSeconds;
            set
            {
                _timerResetSeconds = value;
                MarkAsChanged();
            }
        }

        private float _timerLeftSeconds;
        public float TimerLeftSeconds
        {
            get => _timerLeftSeconds;
            set
            {
                _timerLeftSeconds = value;
                MarkAsChanged();
            }
        }

        public void Update(AnswerTimerData data)
        {
            TimerState = data.TimerState;
            TimerResetSeconds = data.TimerResetSeconds;
            TimerLeftSeconds = data.TimerLeftSeconds;
        }
    }
}