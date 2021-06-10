using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;

namespace Victorina.Commands
{
    public class SendFinalRoundAnswerCommand : PlayerCommand
    {
        [Inject] private FinalRoundData FinalRoundData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        
        public string AnswerText { get; set; }

        public override CommandType Type => CommandType.SendFinalRoundAnswer;

        public override bool CanSendToServer()
        {
            return true;
        }

        public override bool CanExecuteOnServer()
        {
            return Owner == CommandOwner.Player;
        }

        public override void ExecuteOnServer()
        {
            Debug.Log($"Set player '{OwnerPlayer}' answer '{AnswerText}");
            int index = PlayersBoard.GetPlayerIndex(OwnerPlayer);
            FinalRoundData.SetAnswer(index, AnswerText);
        }

        #region Serialization

        public override void Serialize(PooledBitWriter writer)
        {
            writer.WriteString(AnswerText);
        }

        public override void Deserialize(PooledBitReader reader)
        {
            AnswerText = reader.ReadString().ToString();
        }

        #endregion

        public override string ToString()
        {
            return $"[SendFinalRoundAnswerCommand, AnswerText: '{AnswerText}', Owner: {OwnerString}]";
        }
    }
}