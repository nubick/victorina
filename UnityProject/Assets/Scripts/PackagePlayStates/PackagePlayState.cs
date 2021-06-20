using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public abstract class PackagePlayState
    {
        public abstract PlayStateType Type { get; }
        public abstract void Serialize(PooledBitWriter writer);
        public abstract void Deserialize(PooledBitReader reader);
    }
}