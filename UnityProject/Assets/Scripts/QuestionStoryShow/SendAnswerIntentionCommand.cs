using System.Linq;
using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class SendAnswerIntentionCommand : Command, INetworkCommand, IServerCommand
    {
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }
        [Inject] private ShowQuestionSystem ShowQuestionSystem { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        
        public float SpentSeconds { get; set; }
        
        public override CommandType Type => CommandType.SendAnswerIntention;
        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();
        
        public bool CanSend()
        {
            if (PlayStateData.Type != PlayStateType.ShowQuestion)
            {
                Debug.Log($"Wrong intention: Can't send intention in PlayState: {PlayStateData}");
                return false;
            }
            
            if (AnswerTimerData.State == QuestionTimerState.NotStarted)
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

            if (!PlayState.AdmittedPlayersIds.Contains(OwnerPlayer.PlayerId))
            {
                Debug.Log($"Wrong intention: Player '{OwnerString}' is not admitted to answer. Admitted players: {string.Join(", ", PlayState.AdmittedPlayersIds)}");
                return false;
            }

            if (PlayState.WrongAnsweredIds.Contains(OwnerPlayer.PlayerId))
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
            ShowQuestionSystem.PauseTimer();
        }

        public void Serialize(PooledBitWriter writer) => writer.WriteSingle(SpentSeconds);
        public void Deserialize(PooledBitReader reader) => SpentSeconds = reader.ReadSingle();
    }
}