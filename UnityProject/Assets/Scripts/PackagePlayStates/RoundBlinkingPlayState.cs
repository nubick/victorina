using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class RoundBlinkingPlayState : RoundPlayState
    {
        public string QuestionId { get; set; }
        public override PlayStateType Type => PlayStateType.RoundBlinking;
        
        public override void Serialize(PooledBitWriter writer)
        {
            base.Serialize(writer);
            writer.WriteString(QuestionId);
        }

        public override void Deserialize(PooledBitReader reader)
        {
            base.Deserialize(reader);
            QuestionId = reader.ReadString().ToString();
        }
    }
}