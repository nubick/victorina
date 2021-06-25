using System;
using UnityEngine;

namespace Victorina
{
    public class MatchData
    {
        public ReactiveProperty<NetRound> RoundData { get; } = new ReactiveProperty<NetRound>();
        public NetRoundQuestion SelectedRoundQuestion { get; set; }
        
        //Master only
        public int RoundNumber { get; set; }
        
        //Player only data
        public DateTime EnableAnswerTime { get; set; }
        public bool IsMeCurrentPlayer { get; set; }
        public PlayerData ThisPlayer { get; set; }
        
        public MatchData()
        {
            RoundData.Value = new NetRound();
        }

        public void Clear()
        {
            Debug.Log("Master. Clear MatchData");
            RoundData.Value = new NetRound();
            SelectedRoundQuestion = null;
        }
        
        public override string ToString()
        {
            return $"{nameof(RoundData)}: {RoundData.Value}";
        }
    }
}