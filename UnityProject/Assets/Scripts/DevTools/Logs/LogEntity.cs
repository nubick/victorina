using System;
using UnityEngine;

namespace Victorina.DevTools
{
    public class LogEntity
    {
        public LogType Type { get; }
        public string Condition { get; }
        public string StackTrace { get; }
        public DateTime Time { get; }

        public LogEntity(LogType type, string condition, string stackTrace, DateTime time)
        {
            Type = type;
            Condition = condition;
            StackTrace = stackTrace;
            Time = time;
        }
    }
}