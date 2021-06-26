using MLAPI.Serialization.Pooled;
using Victorina.Commands;

namespace Victorina
{
    public class RestartMediaCommand : Command, IPlayerCommand, IServerCommand
    {
        public override CommandType Type => CommandType.RestartMedia;
        public bool CanSend() => true;
        public bool CanExecuteOnServer() => true;
        public void ExecuteOnClient() => Execute();
        public void ExecuteOnServer() => Execute();
        private void Execute() => MetagameEvents.MediaRestarted.Publish();
        public void Serialize(PooledBitWriter writer) { }
        public void Deserialize(PooledBitReader reader) { }
    }
}