using System.Text;
using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;
using Victorina.Commands;

namespace Victorina.DevTools
{
    public class SendPlayerLogsCommand : IndividualPlayerCommand
    {
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private LogsTrackingSystem LogsTrackingSystem { get; set; }
        
        public override CommandType Type => CommandType.SendPlayerLogs;
        public override bool CanSend() => true;

        public SendPlayerLogsCommand() : base(null) { }
        public SendPlayerLogsCommand(PlayerData receiver) : base(receiver) { }
        
        public override void ExecuteOnClient()
        {
            string logs = LogsTrackingSystem.GetLastLogs(300);

            int size = Encoding.Unicode.GetBytes(logs).Length;
            Debug.Log($"logs: {logs.Length}, size: {size}");
            
            CommandsSystem.AddNewCommand(new SavePlayerLogsCommand {Logs = logs});
        }
        
        #region Serialization
        public override void Serialize(PooledBitWriter writer) { }
        public override void Deserialize(PooledBitReader reader) { }
        #endregion
        
        public override string ToString()
        {
            return $"[SendPlayerLogsCommand, Player: {Receiver}, Owner: {OwnerString}]";
        }
    }
}