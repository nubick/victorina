
namespace Victorina.Commands
{
    public abstract class CommandBase
    {
        public abstract CommandType Type { get; }
        public CommandOwner Owner { get; set; }
        public PlayerData OwnerPlayer { get; set; }
        
        protected string OwnerString => Owner == CommandOwner.Master ? "Master" : OwnerPlayer.ToString();
        
        public abstract bool CanExecuteOnServer();
        public abstract void ExecuteOnServer();
    }
}