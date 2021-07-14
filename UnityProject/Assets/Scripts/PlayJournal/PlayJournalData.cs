using System.Collections.Generic;
using Victorina.Commands;

namespace Victorina
{
    public class PlayJournalData
    {
        public List<IServerCommand> ExecutedCommands { get; } = new List<IServerCommand>();
        public bool IsCommandsPlaying { get; set; }
    }
}