using System.Collections.Generic;
using System.Linq;

namespace Victorina
{
    public class QuestionAnswerData
    {
        public MasterIntention MasterIntention { get; set; }
        public ReactiveProperty<QuestionPhase> Phase { get; } = new ReactiveProperty<QuestionPhase>();
        public ReactiveProperty<NetQuestion> SelectedQuestion { get; } = new ReactiveProperty<NetQuestion>();
        public QuestionType QuestionType => SelectedQuestion.Value.Type;
        
        public int CurrentStoryDotIndex { get; set; }
        
        public QuestionTimerState TimerState { get; set; }
        public float TimerResetSeconds { get; set; }
        public float TimerLeftSeconds { get; set; }

        public PlayersButtonClickData PlayersButtonClickData { get; set; } = new PlayersButtonClickData();
        public ReactiveProperty<AuctionData> AuctionData { get; } = new ReactiveProperty<AuctionData>();
        
        public string AnsweringPlayerName { get; set; }
        public byte AnsweringPlayerId { get; set; }
        public HashSet<byte> WrongAnsweredIds { get; set; } = new HashSet<byte>();
        
        public StoryDot CurrentStoryDot => CurrentStory[CurrentStoryDotIndex];
        public StoryDot PreviousStoryDot => CurrentStory[CurrentStoryDotIndex - 1];
        public bool IsLastDot => CurrentStoryDot == CurrentStory.Last();

        public StoryDot[] CurrentStory => Phase.Value == QuestionPhase.ShowAnswer ? 
            SelectedQuestion.Value.AnswerStory : 
            SelectedQuestion.Value.QuestionStory;
        
        //Master Only
        public bool IsAnswerTipEnabled { get; set; }
        public string AnswerTip { get; set; }
        
        public QuestionAnswerData()
        {
            AuctionData.Value = new AuctionData();
        }

        public override string ToString()
        {
            return $"{MasterIntention}, {Phase.Value}, {CurrentStoryDotIndex}, {TimerState}";
        }
    }
}