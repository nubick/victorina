using System;
using UnityEngine;

namespace Victorina
{
    public class MatchData
    {
        public ReactiveProperty<MatchPhase> Phase { get; set; } = new ReactiveProperty<MatchPhase>();
        public ReactiveProperty<NetRoundsInfo> RoundsInfo { get; } = new ReactiveProperty<NetRoundsInfo>();
        public ReactiveProperty<NetRound> RoundData { get; } = new ReactiveProperty<NetRound>();
        public NetRoundQuestion SelectedRoundQuestion { get; set; }
        
        //Question Answering State
        public QuestionAnswerData QuestionAnswerData { get; } = new QuestionAnswerData();
        
        //Master only data
        
        //Player only data
        public DateTime EnableAnswerTime { get; set; }
        public bool IsMeCurrentPlayer { get; set; }
        public PlayerData ThisPlayer { get; set; }
        
        public MatchData()
        {
            RoundsInfo.Value = new NetRoundsInfo();
            RoundData.Value = new NetRound();
        }

        public void Clear()
        {
            Debug.Log("Master. Clear MatchData");
            Phase.Value = MatchPhase.WaitingInLobby;
            RoundsInfo.Value = new NetRoundsInfo();
            RoundData.Value = new NetRound();
            SelectedRoundQuestion = null;
        }

        public string GetTheme()
        {
            return QuestionAnswerData.QuestionType == QuestionType.CatInBag ? 
                QuestionAnswerData.SelectedQuestion.Value.CatInBagInfo.Theme : 
                SelectedRoundQuestion.Theme;
        }

        public override string ToString()
        {
            return $"{nameof(Phase)}: {Phase.Value}, {nameof(RoundData)}: {RoundData.Value}";
        }
    }
}