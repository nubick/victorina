namespace Victorina
{
    public class AnswerTimerData : SyncData
    {
        private QuestionTimerState _state;
        public QuestionTimerState State
        {
            get => _state;
            set
            {
                _state = value; 
                MarkAsChanged();
            }
        }

        private float _resetSeconds;
        public float ResetSeconds
        {
            get => _resetSeconds;
            set
            {
                _resetSeconds = value;
                MarkAsChanged();
            }
        }

        private float _leftSeconds;
        public float LeftSeconds
        {
            get => _leftSeconds;
            set
            {
                _leftSeconds = value;
                MarkAsChanged();
            }
        }

        public void Update(AnswerTimerData data)
        {
            State = data.State;
            ResetSeconds = data.ResetSeconds;
            LeftSeconds = data.LeftSeconds;
        }

        public override string ToString()
        {
            return $"[AnswerTimerData, state: {State}, reset sec: {ResetSeconds}, left sec: {LeftSeconds}]";
        }
    }
}