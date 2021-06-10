using System;
using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class CommandsSystem
    {
        private Injector _injector;
        
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private SendToMasterService SendToMasterService { get; set; }

        public void Initialize(Injector injector)
        {
            _injector = injector;
        }
        
        public void AddNewCommand(CommandBase command)
        {
            _injector.InjectTo(command);
            
            if (NetworkData.IsClient)
            {
                command.Owner = CommandOwner.Player;
                command.OwnerPlayer = MatchData.ThisPlayer;
                
                if (command.CanSendToServer())
                    SendToMasterService.SendCommand(command);
            }
            else if (NetworkData.IsMaster)
            {
                command.Owner = CommandOwner.Master;
                TryExecute(command);
            }
        }
        
        public void AddReceivedPlayerCommand(CommandBase command, PlayerData player)
        {
            _injector.InjectTo(command);

            command.Owner = CommandOwner.Player;
            command.OwnerPlayer = player;
            TryExecute(command);
        }

        private void TryExecute(CommandBase command)
        {
            if (command.CanExecuteOnServer())
            {
#if UNITY_EDITOR
                Debug.Log($"<color=#AAAAFF>EXECUTE: {command}</color>");
#else
                Debug.Log($"EXECUTE: {command}");
#endif
                command.ExecuteOnServer();
            }
        }
        
        public CommandBase Create(CommandType commandType)
        {
            switch (commandType)
            {
                case CommandType.SelectRoundQuestion:
                    return new SelectRoundQuestionCommand();
                default:
                    throw new NotSupportedException($"Command type '{commandType}' is not supported.");
            }
        }
    }
}