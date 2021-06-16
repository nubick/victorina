namespace Victorina.Commands
{
    public class CommandNetworkData
    {
        public INetworkCommand Command { get; }

        public CommandNetworkData(INetworkCommand command)
        {
            Command = command;
        }
    }
}