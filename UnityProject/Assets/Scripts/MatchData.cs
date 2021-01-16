namespace Victorina
{
    public class MatchData
    {
        public ReactiveProperty<MatchPhase> Phase { get; set; } = new ReactiveProperty<MatchPhase>();
        public ReactiveProperty<PlayersBoard> PlayersBoard { get; } = new ReactiveProperty<PlayersBoard>();
        public ReactiveProperty<NetRoundsInfo> RoundsInfo { get; } = new ReactiveProperty<NetRoundsInfo>();
        public ReactiveProperty<NetRound> RoundData { get; } = new ReactiveProperty<NetRound>();
        public NetRoundQuestion SelectedRoundQuestion { get; set; }
        
        public NetQuestion SelectedQuestion { get; set; }
        public ReactiveProperty<int> CurrentStoryDotIndex { get; set; } = new ReactiveProperty<int>();
        public StoryDot CurrentStoryDot => SelectedQuestion.QuestionStory[CurrentStoryDotIndex.Value];

        public MatchData()
        {
            PlayersBoard.Value = new PlayersBoard();
            RoundData.Value = new NetRound();
            RoundsInfo.Value = new NetRoundsInfo();
        }
        
        public override string ToString()
        {
            return $"{nameof(Phase)}: {Phase.Value}, {nameof(PlayersBoard)}: {PlayersBoard.Value}, {nameof(RoundData)}: {RoundData.Value}";
        }
    }
}