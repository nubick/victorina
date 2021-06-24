using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class ShowQuestionPlayState : PackagePlayState
    {
        public NetQuestion NetQuestion { get; set; }
        public int StoryDotIndex { get; set; }
        
        public override PlayStateType Type => PlayStateType.ShowQuestion;
        public StoryDot CurrentStoryDot => NetQuestion.QuestionStory[StoryDotIndex];
        public bool IsLastDot => StoryDotIndex == NetQuestion.QuestionStory.Length - 1;
        
        public override void Serialize(PooledBitWriter writer)
        {
            DataSerializationService.SerializeNetQuestion(writer, NetQuestion);
            writer.WriteInt32(StoryDotIndex);
        }

        public override void Deserialize(PooledBitReader reader)
        {
            NetQuestion = DataSerializationService.DeserializeNetQuestion(reader);
            StoryDotIndex = reader.ReadInt32();
        }
        
        //public StoryDot CurrentStoryDot => CurrentStory[CurrentStoryDotIndex];
        //public StoryDot PreviousStoryDot => CurrentStory[CurrentStoryDotIndex - 1];

        //todo: finish refactoring
        public StoryDot[] CurrentStory => null; //Phase.Value == QuestionPhase.ShowAnswer ? SelectedQuestion.Value.AnswerStory : SelectedQuestion.Value.QuestionStory;
        
        public override string ToString()
        {
            return $"[ShowQuestionPlayState, index: {StoryDotIndex}, storyDot: {CurrentStoryDot}]";
        }
    }
}