using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public abstract class StoryDotPlayState : PackagePlayState
    {
        public NetQuestion NetQuestion { get; set; }
        
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

        protected abstract bool IsQuestionStory { get; }

        private StoryDot[] Story => IsQuestionStory ? NetQuestion.QuestionStory : NetQuestion.AnswerStory;
        public StoryDot CurrentStoryDot => Story[StoryDotIndex];
        public bool IsLastDot => StoryDotIndex == Story.Length - 1;
        public bool IsMediaStoryDot => CurrentStoryDot is AudioStoryDot || CurrentStoryDot is VideoStoryDot;
        
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
    }
}