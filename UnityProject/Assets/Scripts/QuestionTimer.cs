using System;
using UnityEngine;

namespace Victorina
{
    public class QuestionTimer
    {
        private float _resetSeconds;
        private DateTime? _startTime;
        
        public float LeftSeconds { get; set; }
        
        public bool IsRunning => _startTime != null;
        
        public void Reset(float resetSeconds)
        {
            Reset(resetSeconds, resetSeconds);
        }

        public void Reset(float resetSeconds, float leftSeconds)
        {
            _resetSeconds = resetSeconds;
            LeftSeconds = leftSeconds;
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