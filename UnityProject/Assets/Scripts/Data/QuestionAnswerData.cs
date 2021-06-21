using System.Collections.Generic;

namespace Victorina
{
    public class QuestionAnswerData
    {
        public MasterIntention MasterIntention { get; set; }
        public ReactiveProperty<NetQuestion> SelectedQuestion { get; } = new ReactiveProperty<NetQuestion>();
        
        public QuestionTimerState TimerState { get; set; }
        public float TimerResetSeconds { get; set; }
        public float TimerLeftSeconds { get; set; }

        public PlayersButtonClickData PlayersButtonClickData { get; set; } = new PlayersButtonClickData();

        public byte AnsweringPlayerId { get; set; }
        public HashSet<byte> WrongAnsweredIds { get; set; } = new HashSet<byte>();
        public HashSet<byte> AdmittedPlayersIds { get; set; } = new HashSet<byte>();

        public QuestionType QuestionType => SelectedQuestion.Value.Type;

        //Master Only
        public bool IsAnswerTipEnabled { get; set; }
        public string AnswerTip { get; set; }
        
        public override string ToString()
        {
            return $"Intention:{MasterIntention}, timer:{TimerState}";
        }
    }
}