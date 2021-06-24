using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class AcceptingAnswerPlayState : PackagePlayState
    {
        public override PlayStateType Type => PlayStateType.AcceptingAnswer;
        
        public byte AnsweringPlayerId { get; set; }
        public int Price { get; set; }
        public ShowQuestionPlayState ShowQuestionPlayState { get; set; }
        
        public override void Serialize(PooledBitWriter writer) => writer.WriteByte(AnsweringPlayerId);
        public override void Deserialize(PooledBitReader reader) => AnsweringPlayerId = (byte) reader.ReadByte();
    }
}