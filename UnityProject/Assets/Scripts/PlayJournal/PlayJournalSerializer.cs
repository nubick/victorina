using System;
using System.Globalization;
using Commands;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class PlayJournalSerializer
    {
        public const string NotImplemented = "NOT_IMPLEMENTED";
        
        public string ToStringLine(IServerCommand serverCommand)
        {
            Command command = serverCommand as Command;

            if (command == null)
                return $"[ERROR: NOT_COMMAND '{serverCommand}']";

            switch (command.Type)
            {
                case CommandType.RegisterPlayer:
                    return Serialize(command as RegisterPlayerCommand);
                
                case CommandType.MasterMakePlayerAsCurrent:
                    return Serialize(command as MasterMakePlayerAsCurrentCommand);
                case CommandType.MasterUpdatePlayerScore:
                    return Serialize(command as MasterUpdatePlayerScoreCommand);
                case CommandType.MasterUpdatePlayerName:
                    return Serialize(command as MasterUpdatePlayerNameCommand);
                
                case CommandType.SelectRound:
                    return Serialize(command as SelectRoundCommand);
                case CommandType.SelectRoundQuestion:
                    return Serialize(command as SelectRoundQuestionCommand);
                case CommandType.StartRoundQuestion:
                    return SerializeSimpleCommand(command);
                case CommandType.FinishQuestion:
                    return SerializeSimpleCommand(command);
                
                case CommandType.SendAnswerIntention:
                    return Serialize(command as SendAnswerIntentionCommand);
                case CommandType.SelectPlayerForAnswer:
                    return Serialize(command as SelectPlayerForAnswerCommand);
                case CommandType.SelectFastestPlayerForAnswer:
                    return SerializeSimpleCommand(command);
                case CommandType.AcceptAnswerAsCorrect:
                    return SerializeSimpleCommand(command);
                case CommandType.AcceptAnswerAsWrong:
                    return SerializeSimpleCommand(command);
                case CommandType.CancelAcceptingAnswer:
                    return SerializeSimpleCommand(command);
                case CommandType.ShowAnswer:
                    return SerializeSimpleCommand(command);
                
                case CommandType.GiveCatInBag:
                    return Serialize(command as GiveCatInBagCommand);
                case CommandType.FinishCatInBag:
                    return SerializeSimpleCommand(command);
                
                case CommandType.FinishNoRisk:
                    return SerializeSimpleCommand(command);
                
                case CommandType.PassAuction:
                    return SerializeSimpleCommand(command);
                case CommandType.MakeAllInAuction:
                    return SerializeSimpleCommand(command);
                case CommandType.MakeBetAuction:
                    return Serialize(command as MakeBetAuctionCommand);
                case CommandType.MakeBetForPlayerAuction:
                    return Serialize(command as MakeBetForPlayerAuctionCommand);
                case CommandType.FinishAuction:
                    return SerializeSimpleCommand(command);
                
                case CommandType.RemoveFinalRoundTheme:
                    return Serialize(command as RemoveFinalRoundThemeCommand);
                case CommandType.MakeFinalRoundBet:
                    return Serialize(command as MakeFinalRoundBetCommand);
                case CommandType.SendFinalRoundAnswer:
                    return Serialize(command as SendFinalRoundAnswerCommand);
                case CommandType.ClearFinalRoundAnswer:
                    return Serialize(command as ClearFinalRoundAnswerCommand);
                case CommandType.FinishFinalRound:
                    return SerializeSimpleCommand(command);
                
                default:
                    return $"{NotImplemented}: {command.Type} : {serverCommand}";
            }
        }

        private string SerializeInfo(Command command)
        {
            return command.Owner switch
            {
                CommandOwner.Master => $"{command.Type}|{command.Owner}",
                CommandOwner.Player => $"{command.Type}|{command.OwnerPlayer.PlayerId}",
                _ => throw new Exception($"Not supported CommandOwner: {command.Owner}")
            };
        }

        private string SerializeSimpleCommand(Command command)
        {
            return $"{SerializeInfo(command)}";
        }

        private T ReadSimpleCommand<T>() where T: new()
        {
            return new T();
        }
        
        private string Serialize(RegisterPlayerCommand command)
        {
            return $"{SerializeInfo(command)} {command.Guid} {command.Name}";//todo: Name is text!!! Can be any with whitespaces
        }

        private RegisterPlayerCommand ReadRegisterPlayerCommand(string[] parts)
        {
            string guid = parts[1];
            string name = parts[2];
            return new RegisterPlayerCommand {Guid = guid, Name = name};
        }

        private string Serialize(MasterMakePlayerAsCurrentCommand command)
        {
            return $"{SerializeInfo(command)} {command.PlayerId}";
        }

        private MasterMakePlayerAsCurrentCommand ReadMasterMakePlayerAsCurrentCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            return new MasterMakePlayerAsCurrentCommand(playerId);
        }
        
        private string Serialize(MasterUpdatePlayerScoreCommand command)
        {
            return $"{SerializeInfo(command)} {command.PlayerId} {command.NewScore}";
        }

        private MasterUpdatePlayerScoreCommand ReadMasterUpdatePlayerScoreCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            int newScore = int.Parse(parts[2]);
            return new MasterUpdatePlayerScoreCommand(playerId, newScore);
        }
        
        private string Serialize(MasterUpdatePlayerNameCommand command)
        {
            return $"{SerializeInfo(command)} {command.PlayerId} {command.NewPlayerName}";
        }

        private MasterUpdatePlayerNameCommand ReadMasterUpdatePlayerNameCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            string newPlayerName = parts[2];
            return new MasterUpdatePlayerNameCommand(playerId, newPlayerName);
        }
        
        private string Serialize(SelectRoundCommand command)
        {
            return $"{SerializeInfo(command)} {command.RoundNumber}";
        }

        private SelectRoundCommand ReadSelectRoundCommand(string[] parts)
        {
            int roundNumber = int.Parse(parts[1]);
            return new SelectRoundCommand(roundNumber);
        }
        
        private string Serialize(SelectRoundQuestionCommand command)
        {
            return $"{SerializeInfo(command)} {command.QuestionId}";
        }

        private SelectRoundQuestionCommand ReadSelectRoundQuestionCommand(string[] parts)
        {
            string questionId = parts[1];
            return new SelectRoundQuestionCommand {QuestionId = questionId};
        }

        private string Serialize(SendAnswerIntentionCommand command)
        {
            string spentSecondsString = command.SpentSeconds.ToString(CultureInfo.InvariantCulture);
            return $"{SerializeInfo(command)} {spentSecondsString}";
        }

        private SendAnswerIntentionCommand ReadSendAnswerIntentionCommand(string[] parts)
        {
            float spentSeconds = float.Parse(parts[1], CultureInfo.InvariantCulture);
            return new SendAnswerIntentionCommand {SpentSeconds = spentSeconds};
        }

        private string Serialize(SelectPlayerForAnswerCommand command)
        {
            return $"{SerializeInfo(command)} {command.PlayerId}";
        }

        private SelectPlayerForAnswerCommand ReadSelectPlayerForAnswerCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            return new SelectPlayerForAnswerCommand(playerId);
        }

        private string Serialize(GiveCatInBagCommand command)
        {
            return $"{SerializeInfo(command)} {command.ReceiverPlayerId}";
        }

        private GiveCatInBagCommand ReadGiveCatInBagCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            return new GiveCatInBagCommand {ReceiverPlayerId = playerId};
        }

        private string Serialize(MakeBetAuctionCommand command)
        {
            return $"{SerializeInfo(command)} {command.Bet}";
        }

        private MakeBetAuctionCommand ReadMakeBetAuctionCommand(string[] parts)
        {
            int bet = int.Parse(parts[1]);
            return new MakeBetAuctionCommand {Bet = bet};
        }

        private string Serialize(MakeBetForPlayerAuctionCommand command)
        {
            return $"{SerializeInfo(command)} {command.BettingPlayerId} {command.Bet}";
        }

        private MakeBetForPlayerAuctionCommand ReadMakeBetForPlayerAuctionCommand(string[] parts)
        {
            byte bettingPlayerId = byte.Parse(parts[1]);
            int bet = int.Parse(parts[2]);
            return new MakeBetForPlayerAuctionCommand {BettingPlayerId = bettingPlayerId, Bet = bet};
        }

        private string Serialize(RemoveFinalRoundThemeCommand command)
        {
            return $"{SerializeInfo(command)} {command.ThemeIndex}";
        }

        private RemoveFinalRoundThemeCommand ReadRemoveFinalRoundThemeCommand(string[] parts)
        {
            int themeIndex = int.Parse(parts[1]);
            return new RemoveFinalRoundThemeCommand {ThemeIndex = themeIndex};
        }

        private string Serialize(MakeFinalRoundBetCommand command)
        {
            return $"{SerializeInfo(command)} {command.Bet}";
        }

        private MakeFinalRoundBetCommand ReadMakeFinalRoundBetCommand(string[] parts)
        {
            int bet = int.Parse(parts[1]);
            return new MakeFinalRoundBetCommand {Bet = bet};
        }
        
        private string Serialize(SendFinalRoundAnswerCommand command)
        {
            return $"{SerializeInfo(command)} {command.AnswerText}";//todo: how correctly read any text?
        }

        private SendFinalRoundAnswerCommand ReadSendFinalRoundAnswerCommand(string[] parts)
        {
            string answerText = parts[1];//todo: Implement any text reading
            return new SendFinalRoundAnswerCommand {AnswerText = answerText};
        }
        
        private string Serialize(ClearFinalRoundAnswerCommand command)
        {
            return $"{SerializeInfo(command)} {command.PlayerId}";
        }

        private ClearFinalRoundAnswerCommand ReadClearFinalRoundAnswerCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            return new ClearFinalRoundAnswerCommand(playerId);
        }

        private (CommandType, CommandOwner, byte playerId) ReadCommandInfo(string infoString)
        {
            //Possible variants:
            //RegisterPlayer|Master
            //SelectRoundQuestion|1

            string[] parts = infoString.Split('|');

            if (!Enum.TryParse(parts[0], ignoreCase: true, out CommandType commandType))
                throw new Exception($"Can't extract CommandType from '{infoString}'");

            CommandOwner owner = parts[1] == CommandOwner.Master.ToString() ? CommandOwner.Master : CommandOwner.Player;

            byte playerId = 0;
            if (owner == CommandOwner.Player)
            {
                if (!byte.TryParse(parts[1], out playerId))
                    throw new Exception($"Can't extract player id from '{infoString}'");
            }

            return (commandType, owner, playerId);
        }

        public IServerCommand FromStringLine(string line)
        {
            string[] parts = line.Split(' ');

            if (parts.Length == 0)
                throw new Exception($"Line '{line}' doesn't have any parts after split");

            (CommandType CommandType, CommandOwner Owner, byte PlayerId) info = ReadCommandInfo(parts[0]);
            
            IServerCommand serverCommand = ReadCommand(info.CommandType, parts);

            if (serverCommand is Command command)
            {
                command.IsJournal = true;
                command.Owner = info.Owner;
                command.OwnerPlayerId = info.PlayerId;
            }
            else
                Debug.LogWarning($"Can't cast IServerCommand to Command: {serverCommand}");

            return serverCommand;
        }

        private IServerCommand ReadCommand(CommandType commandType, string[] parts)
        {
            return commandType switch
            {
                CommandType.RegisterPlayer => ReadRegisterPlayerCommand(parts),

                CommandType.MasterMakePlayerAsCurrent => ReadMasterMakePlayerAsCurrentCommand(parts),
                CommandType.MasterUpdatePlayerScore => ReadMasterUpdatePlayerScoreCommand(parts),
                CommandType.MasterUpdatePlayerName => ReadMasterUpdatePlayerNameCommand(parts),

                CommandType.SelectRound => ReadSelectRoundCommand(parts),
                CommandType.SelectRoundQuestion => ReadSelectRoundQuestionCommand(parts),
                CommandType.StartRoundQuestion => ReadSimpleCommand<StartRoundQuestionCommand>(),
                CommandType.FinishQuestion => ReadSimpleCommand<FinishQuestionCommand>(),
                
                CommandType.SendAnswerIntention => ReadSendAnswerIntentionCommand(parts),
                CommandType.SelectPlayerForAnswer => ReadSelectPlayerForAnswerCommand(parts),
                CommandType.SelectFastestPlayerForAnswer => ReadSimpleCommand<SelectFastestPlayerForAnswerCommand>(),
                CommandType.AcceptAnswerAsCorrect => ReadSimpleCommand<AcceptAnswerAsCorrectCommand>(),
                CommandType.AcceptAnswerAsWrong => ReadSimpleCommand<AcceptAnswerAsWrongCommand>(),
                CommandType.CancelAcceptingAnswer => ReadSimpleCommand<CancelAcceptingAnswerCommand>(),
                CommandType.ShowAnswer => ReadSimpleCommand<ShowAnswerCommand>(),

                CommandType.GiveCatInBag => ReadGiveCatInBagCommand(parts),
                CommandType.FinishCatInBag => ReadSimpleCommand<FinishCatInBagCommand>(),
                
                CommandType.FinishNoRisk => ReadSimpleCommand<FinishNoRiskCommand>(),
                
                CommandType.PassAuction => ReadSimpleCommand<PassAuctionCommand>(),
                CommandType.MakeAllInAuction => ReadSimpleCommand<MakeAllInAuctionCommand>(),
                CommandType.MakeBetAuction => ReadMakeBetAuctionCommand(parts),
                CommandType.MakeBetForPlayerAuction=>ReadMakeBetForPlayerAuctionCommand(parts),
                CommandType.FinishAuction => ReadSimpleCommand<FinishAuctionCommand>(),
                
                CommandType.RemoveFinalRoundTheme => ReadRemoveFinalRoundThemeCommand(parts),
                CommandType.MakeFinalRoundBet => ReadMakeFinalRoundBetCommand(parts),
                CommandType.SendFinalRoundAnswer => ReadSendFinalRoundAnswerCommand(parts),
                CommandType.ClearFinalRoundAnswer => ReadClearFinalRoundAnswerCommand(parts),
                CommandType.FinishFinalRound => ReadSimpleCommand<FinishFinalRoundCommand>(),

                _ => throw new Exception($"Not supported commandType: {commandType}")
            };
        }
    }
}