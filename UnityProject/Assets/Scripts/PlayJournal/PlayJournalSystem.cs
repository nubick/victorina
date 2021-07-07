using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class PlayJournalSystem
    {
        [Inject] private PlayJournalData Data { get; set; }
        [Inject] private PlayJournalSerializer Serializer { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }

        public void Initialize()
        {
            MetagameEvents.ServerCommandExecuted.Subscribe(OnServerCommandExecuted);
        }

        public void Clear()
        {
            Data.ExecutedCommands.Clear();
        }
        
        private string GetJournalPath(Package package) => $"{package.Path}/journal.txt";

        public bool HasJournal(Package package)
        {
            string journalPath = GetJournalPath(package);
            return File.Exists(journalPath);
        }

        public void Play(Package package)
        {
            Clear();

            string journalPath = GetJournalPath(package);
            if (!File.Exists(journalPath))
                throw new Exception($"Journal doesn't exist by path: '{journalPath}'");

            string journalText = File.ReadAllText(journalPath);
            List<IServerCommand> commands = ReadCommands(journalText);
            CommandsSystem.PlayJournalCommands(commands);

            Data.ExecutedCommands.AddRange(commands);
        }

        private List<IServerCommand> ReadCommands(string journalText)
        {
            List<IServerCommand> commands = new List<IServerCommand>();
            string[] lines = journalText.Split('\n');
            foreach (string line in lines)
            {
                Debug.Log($"Read command line: {line}");

                if (string.IsNullOrWhiteSpace(line))
                    continue;
                
                if (line.StartsWith(PlayJournalSerializer.NotImplemented))
                    continue;

                IServerCommand command = Serializer.FromStringLine(line);
                commands.Add(command);
            }
            return commands;
        }
        
        public void OnServerCommandExecuted(IServerCommand command)
        {
            Data.ExecutedCommands.Add(command);
            SaveToFile(PackageData.Package, Data.ExecutedCommands);
        }
        
        private void SaveToFile(Package package, List<IServerCommand> executedCommands)
        {
            string journalPath = GetJournalPath(package);
            string journalText = GetJournal(executedCommands);
            File.WriteAllText(journalPath, journalText);
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