using System.Collections.Generic;
using System.Linq;
using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class ShowQuestionPlayState : StoryDotPlayState
    {
        public int Price { get; set; }
        public bool IsCameBackFromAcceptingAnswer { get; set; }
        public HashSet<byte> WrongAnsweredIds { get; } = new HashSet<byte>();
        public HashSet<byte> AdmittedPlayersIds { get; } = new HashSet<byte>();
        
        public override PlayStateType Type => PlayStateType.ShowQuestion;
        protected override bool IsQuestionStory => true;

        public override void Serialize(PooledBitWriter writer)
        {
            base.Serialize(writer);
            writer.WriteInt32(Price);
            writer.WriteBool(IsCameBackFromAcceptingAnswer);
            writer.WriteByteArray(WrongAnsweredIds.ToArray());
            writer.WriteByteArray(AdmittedPlayersIds.ToArray());
        }

        public override void Deserialize(PooledBitReader reader)
        {
            base.Deserialize(reader);
            Price = reader.ReadInt32();
            IsCameBackFromAcceptingAnswer = reader.ReadBool();
            WrongAnsweredIds.AddRange(reader.ReadByteArray());
            AdmittedPlayersIds.AddRange(reader.ReadByteArray());
        }
        
        public override string ToString() => $"[ShowQuestionPlayState, index: {StoryDotIndex}, Price: {Price}, StoryDot: {CurrentStoryDot}]";
    }
}