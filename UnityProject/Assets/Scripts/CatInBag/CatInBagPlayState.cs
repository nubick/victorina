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
            //todo: serialize netquestion
        }

        public override void Deserialize(PooledBitReader reader)
        {
            WasGiven = reader.ReadBool();
            //todo: deserialize netquestion
        }
    }
}