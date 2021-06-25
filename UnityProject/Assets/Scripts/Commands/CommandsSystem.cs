using System;
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

        public void Initialize(Injector injector)
        {
            _injector = injector;
        }
        
        public void AddNewCommand(Command command)
        {
            _injector.InjectTo(command);

            if (NetworkData.IsMaster)
                AddNewCommandByMaster(command);
            else if (NetworkData.IsClient)
                AddNewCommandByPlayer(command);
        }

        private void AddNewCommandByMaster(Command command)
        {
            if (command is IndividualPlayerCommand individualPlayerCommand)
            {
                Dev.Log($"SEND: {individualPlayerCommand}", new Color(0.67f, 0.67f, 1f));
                SendToPlayersService.SendCommand(individualPlayerCommand);
            }
            else
            {
                command.Owner = CommandOwner.Master;
                TryExecute(command);
            }
        }

        private void AddNewCommandByPlayer(Command command)
        {
            if (command is INetworkCommand networkCommand)
            {
                command.Owner = CommandOwner.Player;
                command.OwnerPlayer = MatchData.ThisPlayer;

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
            TryExecute(command);
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
            TryExecute(command);
        }

        private void TryExecute(Command command)
        {
            if (command is IndividualPlayerCommand individualPlayerCommand)
            {
                Dev.Log($"EXECUTE: {command}", new Color(0.67f, 0.67f, 1f));
                individualPlayerCommand.ExecuteOnClient();
            }
            else if (command is IServerCommand serverCommand)
            {
                if (serverCommand.CanExecuteOnServer())
                {
                    Dev.Log($"EXECUTE: {command}", new Color(0.67f, 0.67f, 1f));
                    serverCommand.ExecuteOnServer();
                }
                else
                {
                    Dev.Log($"Can't execute command: {command}", new Color(0.67f, 0.67f, 1f));
                }
            }
        }

        public INetworkCommand CreateNetworkCommand(CommandType commandType)
        {
            return commandType switch
            {
                CommandType.SelectRoundQuestion => new SelectRoundQuestionCommand(),
                CommandType.GiveCatInBag => new GiveCatInBagCommand(),
                CommandType.SendAnswerIntention => new SendAnswerIntentionCommand(),
                //Auction
                CommandType.PassAuction => new PassAuctionCommand(),
                CommandType.MakeBetAuction => new MakeBetAuctionCommand(),
                CommandType.MakeAllInAuction => new MakeAllInAuctionCommand(),
                //Final Round
                CommandType.RemoveFinalRoundTheme => new RemoveFinalRoundThemeCommand(),
                CommandType.MakeFinalRoundBet => new MakeFinalRoundBetCommand(),
                CommandType.SendFinalRoundAnswer => new SendFinalRoundAnswerCommand(),
                //Logs
                CommandType.SendPlayerLogs => new SendPlayerLogsCommand(),
                CommandType.SavePlayerLogs => new SavePlayerLogsCommand(),
                _ => throw new NotSupportedException($"Command type '{commandType}' is not supported.")
            };
        }
    }
}