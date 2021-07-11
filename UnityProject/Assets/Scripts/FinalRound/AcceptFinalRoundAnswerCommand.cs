using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class AcceptFinalRoundAnswerCommand : Command, IServerCommand
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        public override CommandType Type => CommandType.AcceptFinalRoundAnswer;

        public byte AcceptingPlayerId { get; }
        public int Bet { get; }
        public bool IsCorrect { get; }

        public AcceptFinalRoundAnswerCommand(byte acceptingPlayerId, int bet, bool isCorrect)
        {
            AcceptingPlayerId = acceptingPlayerId;
            Bet = bet;
            IsCorrect = isCorrect;
        }

        public bool CanExecuteOnServer()
        {
            return PlayStateData.Type == PlayStateType.FinalRound;
        }

        public void ExecuteOnServer()
        {
            if (IsCorrect)
                PlayersBoardSystem.RewardPlayer(AcceptingPlayerId, Bet);
            else
                PlayersBoardSystem.FinePlayer(AcceptingPlayerId, Bet);
        }

        public override string ToString()
        {
            return $"[AcceptFinalRoundAnswerCommand playerId: {AcceptingPlayerId}, bet: {Bet}, correct: {IsCorrect}]";
        }
    }
}