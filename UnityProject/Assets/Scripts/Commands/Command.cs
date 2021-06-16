
namespace Victorina.Commands
{
    public abstract class Command
    {
        public abstract CommandType Type { get; }
        public CommandOwner Owner { get; set; }
        public PlayerData OwnerPlayer { get; set; }
        
        protected string OwnerString => Owner == CommandOwner.Master ? "Master" : OwnerPlayer.ToString();
    }
}