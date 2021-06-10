namespace Victorina.Commands
{
    public class CommandNetworkData
    {
        public CommandBase Command { get; }

        public CommandNetworkData(CommandBase command)
        {
            Command = command;
        }
    }
}