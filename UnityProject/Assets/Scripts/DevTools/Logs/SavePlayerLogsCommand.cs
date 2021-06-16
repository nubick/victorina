using System;
using System.IO;
using Injection;
using MLAPI.Serialization.Pooled;
using Victorina.Commands;

namespace Victorina.DevTools
{
    public class SavePlayerLogsCommand : Command, INetworkCommand, IServerCommand
    {
        [Inject] private PathData PathData { get; set; }
        
        public string Logs { get; set; }
        
        public override CommandType Type => CommandType.SavePlayerLogs;
        public bool CanSend() => true;
        public bool CanExecuteOnServer() => true;

        public void ExecuteOnServer()
        {
            string fileName = $"{OwnerPlayer.Name} - {DateTime.Now:yyyy.MM.dd.hh.mm}.txt";
            string filePath = $"{PathData.LogsPath}/{fileName}";
            File.WriteAllText(filePath, Logs);
        }

        #region Serialization
        
        public void Serialize(PooledBitWriter writer)
        {
            writer.WriteString(Logs);
        }

        public void Deserialize(PooledBitReader reader)
        {
            Logs = reader.ReadString().ToString();
        }

        #endregion
        
        public override string ToString()
        {
            return $"[SavePlayerLogsCommand, Logs size: {Logs.Length}, Owner: {OwnerString}]";
        }
    }
}