namespace Victorina
{
    public class MatchData
    {
        public MatchPhase Phase { get; set; }
        public TextQuestion TextQuestion { get; set; }
        public ReactiveProperty<PlayersBoard> PlayersBoard { get; } = new ReactiveProperty<PlayersBoard>();

        public MatchData()
        {
            PlayersBoard.Value = new PlayersBoard();
        }
        
        public override string ToString()
        {
            return $"{nameof(Phase)}: {Phase}, {nameof(TextQuestion)}: {TextQuestion}, {nameof(PlayersBoard)}: {PlayersBoard}";
        }
    }
}