using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Injection;
using UnityEngine;
using Victorina.Commands;
using Application = UnityEngine.Application;

namespace Victorina.DevTools
{
    public class LogsTrackingSystem
    {
        [Inject] private LogsTrackingData Data { get; set; }
        [Inject] private PathData PathData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        
        public void Initialize()
        {
            Application.logMessageReceived += OnLogMessageReceived;
            Debug.Log("LogsTrackingSystem is initialized");
        }
        
        private void OnLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            Data.AllEntities.Add(new LogEntity(type, condition, stacktrace, DateTime.Now));
            
            if (type == LogType.Exception)
            {
                bool isSpamMode = Data.LastSaveTimesQueue.Count >= 10 && Data.LastSaveTimesQueue.Last() - Data.LastSaveTimesQueue.First() < TimeSpan.FromMinutes(3f);

                if (!isSpamMode)
                    SaveLastExceptionToFile();
                
                Data.LastSaveTimesQueue.Enqueue(DateTime.Now);
                while (Data.LastSaveTimesQueue.Count > 10)
                    Data.LastSaveTimesQueue.Dequeue();
            }
        }

        public string GetAllLogs()
        {
            return ToString(Data.AllEntities);
        }

        public string GetLastLogs(int amount)
        {
            int skip = Mathf.Max(0, Data.AllEntities.Count - amount);
            var entities = Data.AllEntities.Skip(skip).Take(amount);
            return ToString(entities);
        }
        
        private string ToString(IEnumerable<LogEntity> entities)
        {
            StringBuilder sb = new StringBuilder();
            foreach (LogEntity logEntity in entities)
            {
                sb.Append($"{logEntity.Time:hh:mm:ss:ffff} - ");
                switch (logEntity.Type)
                {
                    case LogType.Exception:
                        sb.AppendLine(logEntity.Condition);
                        sb.AppendLine(logEntity.StackTrace);
                        break;
                    default:
                        sb.AppendLine(logEntity.Condition);
                        break;
                }
            }
            return sb.ToString();
        }
        
        private void SaveLastExceptionToFile()
        {
            string logs = GetLastLogs(30);
            string fileName = $"ex_{DateTime.Now:yyyy.MM.dd.hh.mm.ss.ffff}.txt";
            string filePath = $"{PathData.LogsPath}/{fileName}";
            File.WriteAllText(filePath, logs);
            Debug.Log($"Last exception saved to file: {filePath}");

            if (NetworkData.IsClient)
            {
                Debug.Log("Send last exception to Master");
                CommandsSystem.AddNewCommand(new SavePlayerLogsCommand {Logs = logs});
            }
        }

        private void SaveToFile(string filePath)
        {
            Debug.Log($"Save logs to file '{filePath}', entities: {Data.AllEntities.Count}, exceptions:  {Data.ExceptionsAmount}");
            if (File.Exists(filePath))
            {
                Debug.Log($"Can't save logs to file. File exists by path: {filePath}");
            }
            else
            {
                string logs = GetAllLogs();
                File.WriteAllText(filePath, logs);
                Debug.Log("Logs successfully saved.");
            }
        }
    }
}