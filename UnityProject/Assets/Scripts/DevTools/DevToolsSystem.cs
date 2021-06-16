using Injection;
using Victorina.Commands;

namespace Victorina.DevTools
{
    public class DevToolsSystem
    {
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public void RequestPlayerLogs(PlayerData player)
        {
            CommandsSystem.AddNewCommand(new SendPlayerLogsCommand(player));
        }
    }
}