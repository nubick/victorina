using MLAPI.Serialization.Pooled;

namespace Victorina.Commands
{
    public abstract class PlayerCommand : CommandBase
    {
        public abstract bool CanSendToServer();
        public abstract void Serialize(PooledBitWriter writer);
        public abstract void Deserialize(PooledBitReader reader);
    }
}