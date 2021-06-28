using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class SendFinalRoundAnswerCommand : Command, INetworkCommand, IServerCommand
    {
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        public string AnswerText { get; set; }

        public override CommandType Type => CommandType.SendFinalRoundAnswer;
        public bool CanSend() => true;
        private FinalRoundPlayState PlayState => PlayStateData.As<FinalRoundPlayState>();
        
        public bool CanExecuteOnServer()
        {
            return Owner == CommandOwner.Player;
        }

        public void ExecuteOnServer()
        {
            Debug.Log($"Set player '{OwnerPlayer}' answer '{AnswerText}");
            int index = PlayersBoard.GetPlayerIndex(OwnerPlayer);
            PlayState.SetAnswer(index, AnswerText);
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