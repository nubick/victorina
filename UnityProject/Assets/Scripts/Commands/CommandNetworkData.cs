namespace Victorina.Commands
{
    public class CommandNetworkData
    {
        public PlayerCommand Command { get; }

        public CommandNetworkData(PlayerCommand command)
        {
            Command = command;
        }
    }
}