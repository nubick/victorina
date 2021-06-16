using MLAPI.Serialization.Pooled;

namespace Victorina.Commands
{
    public abstract class IndividualPlayerCommand : Command, INetworkCommand
    {
        public PlayerData Receiver { get; }

        protected IndividualPlayerCommand(PlayerData receiver)
        {
            Receiver = receiver;
        }
        
        public abstract void ExecuteOnClient();
        public abstract bool CanSend();
        public abstract void Serialize(PooledBitWriter writer);
        public abstract void Deserialize(PooledBitReader reader);
    }
}