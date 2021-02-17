using System.Collections.Generic;

namespace Victorina
{
    public class QuestionAnswerData
    {
        public MasterIntention MasterIntention { get; set; }
        public ReactiveProperty<QuestionPhase> Phase { get; } = new ReactiveProperty<QuestionPhase>();
        public ReactiveProperty<NetQuestion> SelectedQuestion { get; } = new ReactiveProperty<NetQuestion>();
        
        public int CurrentStoryDotIndex { get; set; }
        
        public QuestionTimerState TimerState { get; set; }
        public float TimerResetSeconds { get; set; }
        public float TimerLeftSeconds { get; set; }
        
        public ReactiveProperty<PlayersButtonClickData> PlayersButtonClickData { get; } = new ReactiveProperty<PlayersButtonClickData>();
        
        public string AnsweringPlayerName { get; set; }
        public ulong AnsweringPlayerId { get; set; }
        public HashSet<ulong> WrongAnsweredIds { get; set; } = new HashSet<ulong>();
        
        public StoryDot CurrentStoryDot => CurrentStory[CurrentStoryDotIndex];

        public StoryDot[] CurrentStory => Phase.Value == QuestionPhase.ShowAnswer ? 
            SelectedQuestion.Value.AnswerStory : 
            SelectedQuestion.Value.QuestionStory;
        
        //Master Only
        public string AnswerTip { get; set; }
        
        public QuestionAnswerData()
        {
            PlayersButtonClickData.Value = new PlayersButtonClickData();
        }
    }
}