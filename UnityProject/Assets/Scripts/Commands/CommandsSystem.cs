using System;
using System.Collections.Generic;
using Injection;
using UnityEngine;
using Victorina.DevTools;

namespace Victorina.Commands
{
    public class CommandsSystem
    {
        private Injector _injector;
        
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private SendToMasterService SendToMasterService { get; set; }
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }

        public void Initialize(Injector injector)
        {
            _injector = injector;
        }
        
        public void AddNewCommand(Command command)
        {
            _injector.InjectTo(command);

            if (NetworkData.IsMaster)
                AddNewCommandByMaster(command);
            
            if (NetworkData.IsClient)
                AddNewCommandByPlayer(command);
        }

        private void AddNewCommandByMaster(Command command)
        {
            command.Owner = CommandOwner.Master;
            
            if (command is IndividualPlayerCommand individualPlayerCommand)
            {
                Dev.Log($"SEND: {individualPlayerCommand}", new Color(0.67f, 0.67f, 1f));
                SendToPlayersService.SendCommand(individualPlayerCommand);
            }
            
            if (command is IPlayerCommand playerCommand)
            {
                Dev.Log($"SEND: {command}", new Color(0.67f, 0.67f, 1f));
                SendToPlayersService.SendCommand(playerCommand);
            }

            if (command is IServerCommand serverCommand)
            {
                TryExecuteOnServer(serverCommand);
            }
        }

        private void AddNewCommandByPlayer(Command command)
        {
            if (command is INetworkCommand networkCommand)
            {
                command.Owner = CommandOwner.Player;
                command.OwnerPlayer = MatchData.ThisPlayer;
                command.OwnerPlayerId = MatchData.ThisPlayer.PlayerId;

                if (networkCommand.CanSend())
                {
                    Dev.Log($"SEND: {networkCommand}", new Color(0.67f, 0.67f, 1f));
                    SendToMasterService.SendCommand(networkCommand);
                }
            }
            else
            {
                throw new Exception($"Client can't create not PlayerCommand: {command}");
            }
        }

        public void AddReceivedPlayerCommand(INetworkCommand networkCommand, PlayerData player)
        {
            Command command = networkCommand as Command;
            if (command == null)
            {
                Debug.Log($"Received player command '{networkCommand}' is null");
                return;
            }

            _injector.InjectTo(command);

            command.Owner = CommandOwner.Player;
            command.OwnerPlayer = player;
            command.OwnerPlayerId = player.PlayerId;

            if (command is IServerCommand serverCommand)
                TryExecuteOnServer(serverCommand);
            else
                throw new Exception($"Not supported command for server execution: {command}");
        }

        public void AddReceivedMasterCommand(INetworkCommand networkCommand)
        {
            Command command = networkCommand as Command;
            if (command == null)
            {
                Debug.Log($"Received master command '{networkCommand}' is null");
                return;
            }

            _injector.InjectTo(command);

            command.Owner = CommandOwner.Master;
            TryExecuteOnClient(command);
        }

        private void TryExecuteOnServer(IServerCommand serverCommand)
        {
            if (serverCommand.CanExecuteOnServer())
            {
                Dev.Log($"EXECUTE: {serverCommand}", new Color(0.67f, 0.67f, 1f));
                serverCommand.ExecuteOnServer();
                MetagameEvents.ServerCommandExecuted.Publish(serverCommand);
            }
            else
            {
                Dev.Log($"Can't execute command: {serverCommand}", new Color(0.67f, 0.67f, 1f));
            }
        }
        
        private void TryExecuteOnClient(Command command)
        {
            if (command is IndividualPlayerCommand individualPlayerCommand)
            {
                Dev.Log($"EXECUTE: {command}", new Color(0.67f, 0.67f, 1f));
                individualPlayerCommand.ExecuteOnClient();
            }
            else if (command is IPlayerCommand playerCommand)
            {
                Dev.Log($"EXECUTE: {command}", new Color(0.67f, 0.67f, 1f));
                playerCommand.ExecuteOnClient();
            }
            else
            {
                throw new Exception($"Not supported command for on client execution: {command}");
            }
        }

        public void PlayJournalCommands(List<IServerCommand> commands)
        {
            foreach (IServerCommand serverCommand in commands)
            {
                _injector.InjectTo(serverCommand);

                if (serverCommand is Command {Owner: CommandOwner.Player} command)
                    command.OwnerPlayer = PlayersBoardSystem.GetPlayer(command.OwnerPlayerId);

                if (serverCommand.CanExecuteOnServer())
                {
                    Dev.Log($"EXECUTE: {serverCommand}", Color.magenta);
                    serverCommand.ExecuteOnServer();
                }
                else
                    throw new Exception($"Journal Command: Can't execute, {serverCommand}");
            }
        }

        public INetworkCommand CreateNetworkCommand(CommandType commandType)
        {
            return commandType switch
            {
                CommandType.SelectRoundQuestion => new SelectRoundQuestionCommand(),
                CommandType.GiveCatInBag => new GiveCatInBagCommand(),
                //Show question
                CommandType.SendAnswerIntention => new SendAnswerIntentionCommand(),
                CommandType.RestartMedia => new RestartMediaCommand(),
                //Auction
                CommandType.PassAuction => new PassAuctionCommand(),
                CommandType.MakeBetAuction => new MakeBetAuctionCommand(),
                CommandType.MakeAllInAuction => new MakeAllInAuctionCommand(),
                //Final Round
                CommandType.RemoveFinalRoundTheme => new RemoveFinalRoundThemeCommand(),
                CommandType.MakeFinalRoundBet => new MakeFinalRoundBetCommand(),
                CommandType.SendFinalRoundAnswer => new SendFinalRoundAnswerCommand(),
                //Effects
                CommandType.PlaySoundEffect => new PlaySoundEffectCommand(),
                //Logs
                CommandType.SendPlayerLogs => new SendPlayerLogsCommand(),
                CommandType.SavePlayerLogs => new SavePlayerLogsCommand(),
                _ => throw new NotSupportedException($"Command type '{commandType}' is not supported.")
            };
        }
    }
}