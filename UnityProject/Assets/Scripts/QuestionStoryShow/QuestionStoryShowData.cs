using System.Linq;

namespace Victorina
{
    public class QuestionStoryShowData : SyncData
    {
        public QuestionStoryShowDataState State { get; set; }
        public int CurrentStoryDotIndex { get; set; }
        
        public StoryDot CurrentStoryDot => CurrentStory[CurrentStoryDotIndex];
        public StoryDot PreviousStoryDot => CurrentStory[CurrentStoryDotIndex - 1];
        public bool IsLastDot => CurrentStoryDot == CurrentStory.Last();

        public StoryDot[] CurrentStory => Phase.Value == QuestionPhase.ShowAnswer ? SelectedQuestion.Value.AnswerStory : SelectedQuestion.Value.QuestionStory;


        public void Update(QuestionStoryShowData data)
        {
            State = data.State;
            CurrentStoryDotIndex = data.CurrentStoryDotIndex;
        }

        public override string ToString()
        {
            return $"State: {State}, Index: {CurrentStoryDotIndex}";
        }
    }

    public enum QuestionStoryShowDataState
    {
        NotActive,
        Question,
        Answer
    }
}