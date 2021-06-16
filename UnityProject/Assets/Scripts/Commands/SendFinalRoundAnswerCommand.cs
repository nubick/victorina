using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;

namespace Victorina.Commands
{
    public class SendFinalRoundAnswerCommand : Command, INetworkCommand, IServerCommand
    {
        [Inject] private FinalRoundData FinalRoundData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        
        public string AnswerText { get; set; }

        public override CommandType Type => CommandType.SendFinalRoundAnswer;
        public bool CanSend() => true;

        public bool CanExecuteOnServer()
        {
            return Owner == CommandOwner.Player;
        }

        public void ExecuteOnServer()
        {
            Debug.Log($"Set player '{OwnerPlayer}' answer '{AnswerText}");
            int index = PlayersBoard.GetPlayerIndex(OwnerPlayer);
            FinalRoundData.SetAnswer(index, AnswerText);
        }

        #region Serialization

        public void Serialize(PooledBitWriter writer)
        {
            writer.WriteString(AnswerText);
        }

        public void Deserialize(PooledBitReader reader)
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