using System;
using System.Globalization;
using Commands;
using Victorina.Commands;

namespace Victorina
{
    public class PlayJournalSerializer
    {
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
                case CommandType.SendAnswerIntention:
                    return Serialize(command as SendAnswerIntentionCommand);
                case CommandType.SelectPlayerForAnswer:
                    return Serialize(command as SelectPlayerForAnswerCommand);
                case CommandType.AcceptAnswerAsCorrect:
                    return Serialize(command as AcceptAnswerAsCorrectCommand);
                case CommandType.AcceptAnswerAsWrong:
                    return Serialize(command as AcceptAnswerAsWrongCommand);
                
                case CommandType.RemoveFinalRoundTheme:
                    return Serialize(command as RemoveFinalRoundThemeCommand);
                case CommandType.MakeFinalRoundBet:
                    return Serialize(command as MakeFinalRoundBetCommand);
                case CommandType.SendFinalRoundAnswer:
                    return Serialize(command as SendFinalRoundAnswerCommand);
                case CommandType.ClearFinalRoundAnswer:
                    return Serialize(command as ClearFinalRoundAnswerCommand);
                
                default:
                    return $"NOT_IMPLEMENTED: {command.Type} : {serverCommand}";
            }
        }

        private string Serialize(RegisterPlayerCommand command)
        {
            return $"{command.Type} {command.Guid} {command.Name}";//todo: Name is text!!! Can be any with whitespaces
        }

        private RegisterPlayerCommand ReadRegisterPlayerCommand(string[] parts)
        {
            string guid = parts[1];
            string name = parts[2];
            return new RegisterPlayerCommand {Guid = guid, Name = name};
        }

        private string Serialize(MasterMakePlayerAsCurrentCommand command)
        {
            return $"{command.Type} {command.PlayerId}";
        }

        private MasterMakePlayerAsCurrentCommand ReadMasterMakePlayerAsCurrentCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            return new MasterMakePlayerAsCurrentCommand(playerId);
        }
        
        private string Serialize(MasterUpdatePlayerScoreCommand command)
        {
            return $"{command.Type} {command.PlayerId} {command.NewScore}";
        }

        private MasterUpdatePlayerScoreCommand ReadMasterUpdatePlayerScoreCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            int newScore = int.Parse(parts[2]);
            return new MasterUpdatePlayerScoreCommand(playerId, newScore);
        }
        
        private string Serialize(MasterUpdatePlayerNameCommand command)
        {
            return $"{command.Type} {command.PlayerId} {command.NewPlayerName}";
        }

        private MasterUpdatePlayerNameCommand ReadMasterUpdatePlayerNameCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            string newPlayerName = parts[2];
            return new MasterUpdatePlayerNameCommand(playerId, newPlayerName);
        }
        
        private string Serialize(SelectRoundCommand command)
        {
            return $"{command.Type} {command.RoundNumber}";
        }

        private SelectRoundCommand ReadSelectRoundCommand(string[] parts)
        {
            int roundNumber = int.Parse(parts[1]);
            return new SelectRoundCommand(roundNumber);
        }
        
        private string Serialize(SelectRoundQuestionCommand command)
        {
            return $"{command.Type} {command.QuestionId}";
        }

        private SelectRoundQuestionCommand ReadSelectRoundQuestionCommand(string[] parts)
        {
            string questionId = parts[1];
            return new SelectRoundQuestionCommand {QuestionId = questionId};
        }
        
        private string Serialize(SendAnswerIntentionCommand command)
        {
            string spentSecondsString = command.SpentSeconds.ToString(CultureInfo.InvariantCulture);
            return $"{command.Type} {spentSecondsString} {command.OwnerPlayer.PlayerId}";
        }

        private SendAnswerIntentionCommand ReadSendAnswerIntentionCommand(string[] parts)
        {
            float spentSeconds = float.Parse(parts[1], CultureInfo.InvariantCulture);
            byte playerId = byte.Parse(parts[2]);
            return new SendAnswerIntentionCommand {SpentSeconds = spentSeconds, OwnerPlayer = null};//todo: Implement
            //return new SendAnswerIntentionCommand {SpentSeconds = spentSeconds, OwnerPlayer = playerId};
        }
        
        private string Serialize(SelectPlayerForAnswerCommand command)
        {
            return $"{command.Type} {command.PlayerId}";
        }

        private SelectPlayerForAnswerCommand ReadSelectPlayerForAnswerCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            return new SelectPlayerForAnswerCommand(playerId);
        }
        
        private string Serialize(AcceptAnswerAsCorrectCommand command)
        {
            return $"{command.Type}";
        }

        private AcceptAnswerAsCorrectCommand ReadAcceptAnswerAsCorrectCommand()
        {
            return new AcceptAnswerAsCorrectCommand();
        }
        
        private string Serialize(AcceptAnswerAsWrongCommand command)
        {
            return $"{command.Type}";
        }

        private AcceptAnswerAsWrongCommand ReadAcceptAnswerAsWrongCommand()
        {
            return new AcceptAnswerAsWrongCommand();
        }

        private string Serialize(RemoveFinalRoundThemeCommand command)
        {
            return $"{command.Type} {command.ThemeIndex}";
        }

        private RemoveFinalRoundThemeCommand ReadRemoveFinalRoundThemeCommand(string[] parts)
        {
            int themeIndex = int.Parse(parts[1]);
            return new RemoveFinalRoundThemeCommand {ThemeIndex = themeIndex};
        }

        private string Serialize(MakeFinalRoundBetCommand command)
        {
            return $"{command.Type} {command.Bet} {command.OwnerPlayer.PlayerId}";
        }

        private MakeFinalRoundBetCommand ReadMakeFinalRoundBetCommand(string[] parts)
        {
            int bet = int.Parse(parts[1]);
            byte playerId = byte.Parse(parts[2]);
            
            return new MakeFinalRoundBetCommand {Bet = bet, OwnerPlayer = null};//todo: implement
            //return new MakeFinalRoundBetCommand {Bet = bet, OwnerPlayer = playerId};
        }
        
        private string Serialize(SendFinalRoundAnswerCommand command)
        {
            return $"{command.Type} {command.OwnerPlayer.PlayerId} {command.AnswerText}";//todo: how correctly read any text?
        }

        private SendFinalRoundAnswerCommand ReadSendFinalRoundAnswerCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            return new SendFinalRoundAnswerCommand {OwnerPlayer = null, AnswerText = ""};//todo: Implement
            //return new SendFinalRoundAnswerCommand {OwnerPlayer = playerId, AnswerText = ""};
        }
        
        private string Serialize(ClearFinalRoundAnswerCommand command)
        {
            return $"{command.Type} {command.PlayerId}";
        }

        private ClearFinalRoundAnswerCommand ReadClearFinalRoundAnswerCommand(string[] parts)
        {
            byte playerId = byte.Parse(parts[1]);
            return new ClearFinalRoundAnswerCommand(playerId);
        }

        
        public IServerCommand FromStringLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new Exception($"Line '{line}' is empty.");

            string[] parts = line.Split(' ');

            if (parts.Length == 0)
                throw new Exception($"Line '{line}' doesn't have any parts after split");

            string commandTypeString = parts[0];

            if (!Enum.TryParse(commandTypeString, ignoreCase: true, out CommandType commandType))
                throw new Exception($"Can't parse '{commandTypeString}' to CommandType enum");

            return ReadCommand(commandType, parts);
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
                CommandType.SendAnswerIntention => ReadSendAnswerIntentionCommand(parts),
                CommandType.SelectPlayerForAnswer => ReadSelectPlayerForAnswerCommand(parts),
                CommandType.AcceptAnswerAsCorrect => ReadAcceptAnswerAsCorrectCommand(),
                CommandType.AcceptAnswerAsWrong => ReadAcceptAnswerAsWrongCommand(),

                CommandType.RemoveFinalRoundTheme => ReadRemoveFinalRoundThemeCommand(parts),
                CommandType.MakeFinalRoundBet => ReadMakeFinalRoundBetCommand(parts),
                CommandType.SendFinalRoundAnswer => ReadSendFinalRoundAnswerCommand(parts),
                CommandType.ClearFinalRoundAnswer => ReadClearFinalRoundAnswerCommand(parts),

                _ => throw new Exception($"Not supported commandType: {commandType}")
            };
        }
    }
}