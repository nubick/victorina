using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class CatInBagPlayState : PackagePlayState
    {
        public bool WasGiven { get; set; }
        public NetQuestion NetQuestion { get; set; }
        
        public override PlayStateType Type => PlayStateType.CatInBag;
        
        public override void Serialize(PooledBitWriter writer)
        {
            writer.WriteBool(WasGiven);
            DataSerializationService.SerializeNetQuestion(writer, NetQuestion);
        }

        public override void Deserialize(PooledBitReader reader)
        {
            WasGiven = reader.ReadBool();
            NetQuestion = DataSerializationService.DeserializeNetQuestion(reader);
        }

        public override string ToString()
        {
            return $"[CatInBagPlayState, {nameof(WasGiven)}: {WasGiven}, {nameof(NetQuestion)}: {NetQuestion}";
        }
    }
}