using System;
using System.Linq;
using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class PlayerAnswerSystem : IKeyPressedHandler
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }
        
        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();
        
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
            return CanSendAnswerIntention() && AnswerTimerData.State != QuestionTimerState.NotStarted;
        }
        
        public bool CanSendAnswerIntention()
        {
            if (NetworkData.IsMaster ||
                PlayStateData.Type != PlayStateType.ShowQuestion ||
                WasIntentionSent() ||
                WasWrongAnswer() ||
                !IsAdmitted())
                return false;

            return true;
        }

        public bool WasWrongAnswer()
        {
            return PlayState.WrongAnsweredIds.Contains(NetworkData.RegisteredPlayerId);
        }

        public bool WasIntentionSent()
        {
            return PlayersButtonClickData.Players.Any(_ => _.PlayerId == NetworkData.RegisteredPlayerId);
        }

        public bool IsAdmitted()
        {
            return PlayState.AdmittedPlayersIds.Contains(NetworkData.RegisteredPlayerId);
        }
    }
}