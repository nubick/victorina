using Injection;
using Victorina.Commands;

namespace Victorina.DevTools
{
    public class DevToolsSystem
    {
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private DevToolsData DevToolsData { get; set; }

        public void Initialize()
        {
            DevToolsData.InGameDebugConsole.SetActive(Static.BuildMode == BuildMode.Development);
        }
        
        public void RequestPlayerLogs(PlayerData player)
        {
            CommandsSystem.AddNewCommand(new SendPlayerLogsCommand(player));
        }

        public void ActivateGameDebugConsole()
        {
            DevToolsData.InGameDebugConsole.SetActive(true);
        }
    }
}