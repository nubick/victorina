using System.Collections.Generic;
using System.Linq;
using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class ShowQuestionPlayState : PackagePlayState
    {
        public NetQuestion NetQuestion { get; set; }
        public int Price { get; set; }
        
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
        
        public bool IsCameBackFromAcceptingAnswer { get; set; }
        
        public HashSet<byte> WrongAnsweredIds { get; } = new HashSet<byte>();
        public HashSet<byte> AdmittedPlayersIds { get; } = new HashSet<byte>();
        
        public override PlayStateType Type => PlayStateType.ShowQuestion;
        public StoryDot CurrentStoryDot => NetQuestion.QuestionStory[StoryDotIndex];
        public bool IsLastDot => StoryDotIndex == NetQuestion.QuestionStory.Length - 1;
        
        public override void Serialize(PooledBitWriter writer)
        {
            DataSerializationService.SerializeNetQuestion(writer, NetQuestion);
            writer.WriteInt32(Price);
            writer.WriteInt32(StoryDotIndex);
            writer.WriteBool(IsCameBackFromAcceptingAnswer);
            writer.WriteByteArray(WrongAnsweredIds.ToArray());
            writer.WriteByteArray(AdmittedPlayersIds.ToArray());
        }

        public override void Deserialize(PooledBitReader reader)
        {
            NetQuestion = DataSerializationService.DeserializeNetQuestion(reader);
            Price = reader.ReadInt32();
            StoryDotIndex = reader.ReadInt32();
            IsCameBackFromAcceptingAnswer = reader.ReadBool();
            WrongAnsweredIds.AddRange(reader.ReadByteArray());
            AdmittedPlayersIds.AddRange(reader.ReadByteArray());
        }
        
        public override string ToString() => $"[ShowQuestionPlayState, index: {StoryDotIndex}, Price: {Price}, StoryDot: {CurrentStoryDot}]";
    }
}