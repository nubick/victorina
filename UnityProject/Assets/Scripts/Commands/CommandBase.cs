using System;
using MLAPI.Serialization.Pooled;

namespace Victorina.Commands
{
    public abstract class CommandBase
    {
        public abstract CommandType Type { get; }
        public CommandOwner Owner { get; set; }
        public PlayerData OwnerPlayer { get; set; }
        
        protected string OwnerString => Owner == CommandOwner.Master ? "Master" : OwnerPlayer.ToString();
        
        public abstract bool CanSendToServer();
        public abstract bool CanExecuteOnServer();
        public abstract void ExecuteOnServer();

        public virtual void Serialize(PooledBitWriter writer)
        {
            throw new NotImplementedException($"Command '{GetType()}' doesn't have Serialize method");
        }

        public virtual void Deserialize(PooledBitReader reader)
        {
            throw new NotImplementedException($"Command '{GetType()}' doesn't have Deserialize method");
        }
    }
}