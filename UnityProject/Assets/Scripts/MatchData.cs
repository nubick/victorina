namespace Victorina
{
    public class MatchData
    {
        public ReactiveProperty<MatchPhase> Phase { get; set; } = new ReactiveProperty<MatchPhase>();
        public ReactiveProperty<PlayersBoard> PlayersBoard { get; } = new ReactiveProperty<PlayersBoard>();
        public ReactiveProperty<NetRound> RoundData { get; } = new ReactiveProperty<NetRound>();
        public NetRoundQuestion SelectedQuestion { get; set; }
        
        public MatchData()
        {
            PlayersBoard.Value = new PlayersBoard();
            RoundData.Value = new NetRound();
        }
        
        public override string ToString()
        {
            return $"{nameof(Phase)}: {Phase.Value}, {nameof(PlayersBoard)}: {PlayersBoard.Value}, {nameof(RoundData)}: {RoundData.Value}";
        }
    }
}