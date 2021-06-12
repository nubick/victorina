using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Victorina.DevTools
{
    public class LogsTrackingData
    {
        public List<LogEntity> AllEntities { get; } = new List<LogEntity>();
        public Queue<DateTime> LastSaveTimesQueue { get; } = new Queue<DateTime>();
        
        public int ExceptionsAmount => AllEntities.Count(_ => _.Type == LogType.Exception);
        public bool WasAnyExceptions => ExceptionsAmount > 0;
    }
}