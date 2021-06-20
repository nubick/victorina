using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class RoundPlayState : PackagePlayState
    {
        public int RoundNumber { get; set; }
        public RoundType[] RoundTypes { get; set; }
        public NetRound NetRound { get; set; }
        
        public override PlayStateType Type => PlayStateType.Round;
        
        public override void Serialize(PooledBitWriter writer)
        {
            writer.WriteInt32(RoundNumber);
            SerializationTools.SerializeEnumArray(writer, RoundTypes);
            DataSerializationService.SerializeNetRound(writer, NetRound);
        }

        public override void Deserialize(PooledBitReader reader)
        {
            RoundNumber = reader.ReadInt32();
            RoundTypes = SerializationTools.DeserializeEnumArray<RoundType>(reader);
            NetRound = DataSerializationService.DeserializeNetRound(reader);
        }
        
        public override string ToString()
        {
            return $"[RoundPlayState, {nameof(RoundNumber)}: {RoundNumber}]";
        }
    }
}