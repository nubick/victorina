using System;
using UnityEngine;

namespace Victorina
{
    public class QuestionTimer
    {
        private float _resetSeconds;
        private DateTime? _startTime;
        
        public float LeftSeconds { get; set; }
        
        public void Reset(float seconds)
        {
            _resetSeconds = seconds;
            LeftSeconds = seconds;
        }
        
        public void Start()
        {
            _startTime = DateTime.Now;   
        }

        public void Stop()
        {
            if (_startTime != null)
            {
                LeftSeconds = GetNowLeftSeconds();
                _startTime = null;
            }
        }

        private float GetNowLeftSeconds()
        {
            float secondsPassed = (float) (DateTime.Now - _startTime.Value).TotalSeconds;
            return Mathf.Max(0f, LeftSeconds - secondsPassed);
        }
        
        public float GetLeftSecondsPercentage()
        {
            float leftSeconds = _startTime == null ? LeftSeconds : GetNowLeftSeconds();
            return Mathf.Clamp01(leftSeconds / _resetSeconds);
        }
    }
}