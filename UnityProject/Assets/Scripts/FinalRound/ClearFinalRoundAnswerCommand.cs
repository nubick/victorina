using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class ClearFinalRoundAnswerCommand : Command, IServerCommand
    {
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        
        public byte PlayerId { get; }
        
        public override CommandType Type => CommandType.ClearFinalRoundAnswer;
        private FinalRoundPlayState PlayState => PlayStateData.As<FinalRoundPlayState>();
        private PlayerData Player => PlayersBoardSystem.GetPlayer(PlayerId);
        
        public ClearFinalRoundAnswerCommand(byte playerId)
        {
            PlayerId = playerId;
        }

        public bool CanExecuteOnServer()
        {
            if (!FinalRoundSystem.CanParticipate(Player))
            {
                Debug.Log($"Can't clear player {Player} answer. Player doesn't participate in Final Round.");
                return false;
            }
            
            int index = PlayersBoard.GetPlayerIndex(Player);
            if (!PlayState.DoneAnswers[index])
            {
                Debug.Log($"Can't clear player {Player} answer. There is no any answers yet from this player.");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            int index = PlayersBoard.GetPlayerIndex(Player);
            PlayState.ClearAnswer(index);
        }
        
        public override string ToString()
        {
            return $"[ClearFinalRoundAnswerCommand, Owner: {OwnerString}]";
        }
    }
}