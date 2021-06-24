using System.Linq;
using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class SendAnswerIntentionCommand : Command, INetworkCommand, IServerCommand
    {
        [Inject] private QuestionAnswerData Data { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        
        public float SpentSeconds { get; set; }
        
        public override CommandType Type => CommandType.SendAnswerIntention;
        
        public bool CanSend()
        {
            if (Data.TimerState == QuestionTimerState.NotStarted)
            {
                Debug.Log("Wrong intention: Timer is not started.");
                return false;
            }

            bool wasReceivedBefore = PlayersButtonClickData.Players.Any(_ => _.PlayerId == OwnerPlayer.PlayerId);
            if (wasReceivedBefore)
            {
                Debug.Log($"Wrong intention: Server has player '{OwnerString}' intention. This is another one.");
                return false;
            }

            if (!Data.AdmittedPlayersIds.Contains(OwnerPlayer.PlayerId))
            {
                Debug.Log($"Wrong intention: Player '{OwnerString}' is not admitted to answer. Admitted players: {string.Join(", ", Data.AdmittedPlayersIds)}");
                return false;
            }

            if (Data.WrongAnsweredIds.Contains(OwnerPlayer.PlayerId))
            {
                Debug.Log($"Wrong intention: Player '{OwnerString}' answered wrongly before.");
                return false;
            }
            
            return true;
        }
        
        public bool CanExecuteOnServer()
        {
            return CanSend();
        }

        public void ExecuteOnServer()
        {
            PlayersButtonClickData.Add(OwnerPlayer.PlayerId, OwnerPlayer.Name, SpentSeconds);
            QuestionAnswerSystem.PauseTimer();
        }

        public void Serialize(PooledBitWriter writer) => writer.WriteSingle(SpentSeconds);
        public void Deserialize(PooledBitReader reader) => SpentSeconds = reader.ReadSingle();
    }
}