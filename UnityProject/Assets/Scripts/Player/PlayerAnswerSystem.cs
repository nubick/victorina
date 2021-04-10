using System;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayerAnswerSystem : IKeyPressedHandler
    {
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private SendToMasterService SendToMasterService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        
        public void StartTimer(float resetSeconds, float leftSeconds)
        {
            MatchData.EnableAnswerTime = DateTime.UtcNow;
            QuestionTimer.Reset(resetSeconds, leftSeconds);
            QuestionTimer.Start();
        }
        
        public void StopTimer()
        {
            QuestionTimer.Stop();
        }

        private void SendAnswerIntention()
        {
            if (!CanSendAnswerIntention())
                return;
            
            float spentSeconds = (float) (DateTime.UtcNow - MatchData.EnableAnswerTime).TotalSeconds;
            SendToMasterService.SendPlayerButton(spentSeconds);
        }

        public void OnAnswerButtonClicked()
        {
            SendAnswerIntention();
        }
        
        public void OnKeyPressed(KeyCode keyCode)
        {
            if(keyCode == KeyCode.Space)
                SendAnswerIntention();
        }
        
        public bool CanSendAnswerIntention()
        {
            if (NetworkData.IsMaster ||
                MatchData.Phase.Value != MatchPhase.Question ||
                QuestionAnswerData.Phase.Value != QuestionPhase.ShowQuestion ||
                QuestionAnswerData.TimerState == QuestionTimerState.NotStarted ||
                WasIntentionSent())
                return false;

            switch (QuestionAnswerData.QuestionType)
            {
                case QuestionType.Simple:
                    return !WasWrongAnswer();
                case QuestionType.NoRisk:
                case QuestionType.CatInBag:
                case QuestionType.Auction:
                    return MatchData.IsMeCurrentPlayer;
                default:
                    throw new Exception($"Not supported QuestionType: {QuestionAnswerData.QuestionType}");
            }
        }

        public bool WasWrongAnswer()
        {
            return QuestionAnswerData.WrongAnsweredIds.Contains(NetworkData.RegisteredPlayerId);
        }

        public bool WasIntentionSent()
        {
            return QuestionAnswerData.PlayersButtonClickData.Value.Players.Any(_ => _.PlayerId == NetworkData.RegisteredPlayerId);
        } 
    }
}