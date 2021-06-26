using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class ShowAnswerPlayState : PackagePlayState
    {
        private int _storyDotIndex;
        public int StoryDotIndex
        {
            get => _storyDotIndex;
            set
            {
                _storyDotIndex = value;
                MarkAsChanged();
            }
        }
        
        public NetQuestion NetQuestion { get; set; }

        public override PlayStateType Type => PlayStateType.ShowAnswer;
        public StoryDot CurrentStoryDot => NetQuestion.AnswerStory[StoryDotIndex];
        public bool IsLastDot => StoryDotIndex == NetQuestion.AnswerStory.Length - 1;
        
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

        public override string ToString() => $"[ShowAnswerPlayState, index: {StoryDotIndex}, question: {NetQuestion}]";
    }
}