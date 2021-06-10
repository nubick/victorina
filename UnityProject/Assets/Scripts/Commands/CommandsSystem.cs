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
                if (command is PlayerCommand playerCommand)
                {
                    playerCommand.Owner = CommandOwner.Player;
                    playerCommand.OwnerPlayer = MatchData.ThisPlayer;

                    if (playerCommand.CanSendToServer())
                    {
#if UNITY_EDITOR
                        Debug.Log($"<color=#AAAAFF>SEND: {playerCommand}</color>");
#else
                        Debug.Log($"SEND: {playerCommand}");
#endif
                        SendToMasterService.SendCommand(playerCommand);
                    }
                }
                else
                {
                    throw new Exception($"Client can't create not PlayerCommand: {command}");
                }
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
        
        public PlayerCommand CreatePlayerCommand(CommandType commandType)
        {
            switch (commandType)
            {
                case CommandType.SelectRoundQuestion:
                    return new SelectRoundQuestionCommand();
                case CommandType.RemoveFinalRoundTheme:
                    return new RemoveFinalRoundThemeCommand();
                case CommandType.MakeFinalRoundBet:
                    return new MakeFinalRoundBetCommand();
                case CommandType.SendFinalRoundAnswer:
                    return new SendFinalRoundAnswerCommand();
                default:
                    throw new NotSupportedException($"Command type '{commandType}' is not supported.");
            }
        }
    }
}