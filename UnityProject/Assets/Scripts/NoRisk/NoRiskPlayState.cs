using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class NoRiskPlayState : PackagePlayState
    {
        public override PlayStateType Type => PlayStateType.NoRisk;
        
        public override void Serialize(PooledBitWriter writer) { }
        public override void Deserialize(PooledBitReader reader) { }
    }
}