using MLAPI.Serialization.Pooled;

namespace Victorina.Commands
{
    public interface INetworkCommand
    {
        CommandType Type { get; }
        bool CanSend();
        void Serialize(PooledBitWriter writer);
        void Deserialize(PooledBitReader reader);
    }
}