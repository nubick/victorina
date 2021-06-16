using Injection;
using UnityEngine;

namespace Victorina.Commands
{
    public class ClearFinalRoundAnswerCommand : Command, IServerCommand
    {
        [Inject] private FinalRoundData FinalRoundData { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        
        public PlayerData Player { get; set; }
        
        public override CommandType Type => CommandType.ClearFinalRoundAnswer;
        
        public bool CanExecuteOnServer()
        {
            if (!FinalRoundSystem.CanParticipate(Player))
            {
                Debug.Log($"Can't clear player {Player} answer. Player doesn't participate in Final Round.");
                return false;
            }
            
            int index = PlayersBoard.GetPlayerIndex(Player);
            if (!FinalRoundData.DoneAnswers[index])
            {
                Debug.Log($"Can't clear player {Player} answer. There is no any answers yet from this player.");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            int index = PlayersBoard.GetPlayerIndex(Player);
            FinalRoundData.ClearAnswer(index);
        }
        
        public override string ToString()
        {
            return $"[ClearFinalRoundAnswerCommand, Owner: {OwnerString}]";
        }
    }
}