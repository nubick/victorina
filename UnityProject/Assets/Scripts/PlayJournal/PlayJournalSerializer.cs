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
                default:
                    return $"NOT_IMPLEMENTED: {command.Type} : {serverCommand}";
            }
        }

        private string Serialize(MasterMakePlayerAsCurrentCommand command)
        {
            return $"{command.Type} {command.Player.PlayerId}";
        }

        private string Serialize(MasterUpdatePlayerScoreCommand command)
        {
            return $"{command.Type} {command.Player.PlayerId} {command.NewScore}";
        }

        private string Serialize(MasterUpdatePlayerNameCommand command)
        {
            return $"{command.Type} {command.Player.PlayerId} {command.NewPlayerName}";
        }

        private string Serialize(SelectRoundCommand command)
        {
            return $"{command.Type} {command.RoundNumber}";
        }

        private string Serialize(SelectRoundQuestionCommand command)
        {
            return $"{command.Type} {command.QuestionId}";
        }

        private string Serialize(SendAnswerIntentionCommand command)
        {
            return $"{command.Type} {command.SpentSeconds} {command.OwnerPlayer.PlayerId}";
        }

        private string Serialize(SelectPlayerForAnswerCommand command)
        {
            return $"{command.Type} {command.PlayerId}";
        }

        private string Serialize(AcceptAnswerAsCorrectCommand command)
        {
            return $"{command.Type}";
        }

        private string Serialize(AcceptAnswerAsWrongCommand command)
        {
            return $"{command.Type}";
        }
    }
}