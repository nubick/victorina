using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class ResultPlayState : PackagePlayState
    {
        public override PlayStateType Type => PlayStateType.Result;
        public override void Serialize(PooledBitWriter writer) { }
        public override void Deserialize(PooledBitReader reader) { }
    }
}