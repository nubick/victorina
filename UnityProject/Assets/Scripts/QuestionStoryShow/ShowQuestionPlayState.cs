using System.Collections.Generic;
using System.Linq;
using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class ShowQuestionPlayState : PackagePlayState
    {
        public NetQuestion NetQuestion { get; set; }
        public int StoryDotIndex { get; set; }
        
        public HashSet<byte> WrongAnsweredIds { get; } = new HashSet<byte>();
        public HashSet<byte> AdmittedPlayersIds { get; } = new HashSet<byte>();
        
        public override PlayStateType Type => PlayStateType.ShowQuestion;
        public StoryDot CurrentStoryDot => NetQuestion.QuestionStory[StoryDotIndex];
        public bool IsLastDot => StoryDotIndex == NetQuestion.QuestionStory.Length - 1;
        
        public override void Serialize(PooledBitWriter writer)
        {
            DataSerializationService.SerializeNetQuestion(writer, NetQuestion);
            writer.WriteInt32(StoryDotIndex);
            writer.WriteByteArray(WrongAnsweredIds.ToArray());
            writer.WriteByteArray(AdmittedPlayersIds.ToArray());
        }

        public override void Deserialize(PooledBitReader reader)
        {
            NetQuestion = DataSerializationService.DeserializeNetQuestion(reader);
            StoryDotIndex = reader.ReadInt32();
            WrongAnsweredIds.AddRange(reader.ReadByteArray());
            AdmittedPlayersIds.AddRange(reader.ReadByteArray());
        }
        
        public override string ToString() => $"[ShowQuestionPlayState, index: {StoryDotIndex}, storyDot: {CurrentStoryDot}]";
    }
}