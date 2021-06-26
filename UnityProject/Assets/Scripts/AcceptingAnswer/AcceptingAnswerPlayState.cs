using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class AcceptingAnswerPlayState : PackagePlayState
    {
        public override PlayStateType Type => PlayStateType.AcceptingAnswer;
        
        public byte AnsweringPlayerId { get; set; }
        public ShowQuestionPlayState ShowQuestionPlayState { get; set; }
        
        public override void Serialize(PooledBitWriter writer)
        {
            writer.WriteByte(AnsweringPlayerId);
            ShowQuestionPlayState.Serialize(writer);
        }

        public override void Deserialize(PooledBitReader reader)
        {
            AnsweringPlayerId = (byte) reader.ReadByte();
            ShowQuestionPlayState = new ShowQuestionPlayState();
            ShowQuestionPlayState.Deserialize(reader);
        }

        public override string ToString()
        {
            return $"[AcceptingAnswerPlayState, AnsweringPlayerId: {AnsweringPlayerId}]";
        }
    }
}