using System;

namespace Victorina
{
    public class MatchData
    {
        //Master only
        public int RoundNumber { get; set; }
        
        //Player only data
        public DateTime EnableAnswerTime { get; set; }
        public bool IsMeCurrentPlayer { get; set; }
        public PlayerData ThisPlayer { get; set; }
    }
}