using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class ShowAnswerPlayState : PackagePlayState
    {
        public int StoryDotIndex { get; set; }
        public NetQuestion NetQuestion { get; set; }

        public StoryDot CurrentStoryDot => NetQuestion.AnswerStory[StoryDotIndex];
        
        public override PlayStateType Type => PlayStateType.ShowAnswer;
        
        public override void Serialize(PooledBitWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public override void Deserialize(PooledBitReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}