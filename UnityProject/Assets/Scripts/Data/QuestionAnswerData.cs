using System.Collections.Generic;

namespace Victorina
{
    public class QuestionAnswerData
    {
        public ReactiveProperty<QuestionPhase> Phase { get; } = new ReactiveProperty<QuestionPhase>();
        public ReactiveProperty<NetQuestion> SelectedQuestion { get; } = new ReactiveProperty<NetQuestion>();
        public ReactiveProperty<int> CurrentStoryDotIndex { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<PlayersButtonClickData> PlayersButtonClickData { get; } = new ReactiveProperty<PlayersButtonClickData>();
        public string AnsweringPlayerName { get; set; }
        public ulong AnsweringPlayerId { get; set; }
        public HashSet<ulong> WrongAnsweredIds { get; set; } = new HashSet<ulong>();
        
        public StoryDot CurrentStoryDot => CurrentStory[CurrentStoryDotIndex.Value];

        public StoryDot[] CurrentStory => Phase.Value == QuestionPhase.ShowAnswer ? 
            SelectedQuestion.Value.AnswerStory : 
            SelectedQuestion.Value.QuestionStory;

        //Master Only
        public bool WasTimerStarted { get; set; }
        public bool IsTimerOn { get; set; }
        public string AnswerTip { get; set; }
        
        public QuestionAnswerData()
        {
            PlayersButtonClickData.Value = new PlayersButtonClickData();
        }
    }
}