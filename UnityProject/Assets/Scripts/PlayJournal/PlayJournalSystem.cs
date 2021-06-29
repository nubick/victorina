using System.Collections.Generic;
using System.IO;
using System.Text;
using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class PlayJournalSystem
    {
        [Inject] private PlayJournalData Data { get; set; }
        [Inject] private PlayJournalSerializer Serializer { get; set; }
        [Inject] private PackageData PackageData { get; set; }

        public void Initialize()
        {
            MetagameEvents.ServerCommandExecuted.Subscribe(OnServerCommandExecuted);
        }
        
        public void OnServerCommandExecuted(IServerCommand command)
        {
            Data.ExecutedCommands.Add(command);
            SaveToFile(PackageData.Package, Data.ExecutedCommands);
        }
        
        private void SaveToFile(Package package, List<IServerCommand> executedCommands)
        {
            string path = $"{package.Path}/journal.txt";
            string journal = GetJournal(executedCommands);
            File.WriteAllText(path, journal);
        }
        
        private string GetJournal(List<IServerCommand> executedCommands)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IServerCommand serverCommand in executedCommands)
                sb.AppendLine(Serializer.ToStringLine(serverCommand));
            return sb.ToString();
        }
    }
}