using System;
using System.Linq;
using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class PlayerAnswerSystem : IKeyPressedHandler
    {
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private PackagePlayStateData PackagePlayStateData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        
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
            float spentSeconds = (float) (DateTime.UtcNow - MatchData.EnableAnswerTime).TotalSeconds;
            CommandsSystem.AddNewCommand(new SendAnswerIntentionCommand {SpentSeconds = spentSeconds});
        }

        public void OnAnswerButtonClicked()
        {
            if (CanSendAnswerIntentionNow())
                SendAnswerIntention();
        }
        
        public void OnKeyPressed(KeyCode keyCode)
        {
            if(keyCode == KeyCode.Space && CanSendAnswerIntentionNow())
                SendAnswerIntention();
        }

        public bool CanSendAnswerIntentionNow()
        {
            return CanSendAnswerIntention() && QuestionAnswerData.TimerState != QuestionTimerState.NotStarted;
        }
        
        public bool CanSendAnswerIntention()
        {
            if (NetworkData.IsMaster ||
                PackagePlayStateData.Type != PlayStateType.ShowQuestion ||
                WasIntentionSent() ||
                WasWrongAnswer() ||
                !IsAdmitted())
                return false;

            return true;
        }

        public bool WasWrongAnswer()
        {
            return QuestionAnswerData.WrongAnsweredIds.Contains(NetworkData.RegisteredPlayerId);
        }

        public bool WasIntentionSent()
        {
            return PlayersButtonClickData.Players.Any(_ => _.PlayerId == NetworkData.RegisteredPlayerId);
        }

        public bool IsAdmitted()
        {
            return QuestionAnswerData.AdmittedPlayersIds.Contains(NetworkData.RegisteredPlayerId);
        }
    }
}