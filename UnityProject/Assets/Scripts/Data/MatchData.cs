
namespace Victorina
{
    public class MatchData
    {
        public ReactiveProperty<MatchPhase> Phase { get; set; } = new ReactiveProperty<MatchPhase>();
        public ReactiveProperty<PlayersBoard> PlayersBoard { get; } = new ReactiveProperty<PlayersBoard>();
        public ReactiveProperty<NetRoundsInfo> RoundsInfo { get; } = new ReactiveProperty<NetRoundsInfo>();
        public ReactiveProperty<NetRound> RoundData { get; } = new ReactiveProperty<NetRound>();
        public NetRoundQuestion SelectedRoundQuestion { get; set; }
        
        //Question Answering State
        public QuestionAnswerData QuestionAnswerData { get; } = new QuestionAnswerData();
        
        //Master only data
        
        //Player only data
        public PlayerMatchData Player { get; set; }
        
        public MatchData()
        {
            PlayersBoard.Value = new PlayersBoard();
            RoundData.Value = new NetRound();
            RoundsInfo.Value = new NetRoundsInfo();
            Player = new PlayerMatchData();
        }
        
        public override string ToString()
        {
            return $"{nameof(Phase)}: {Phase.Value}, {nameof(PlayersBoard)}: {PlayersBoard.Value}, {nameof(RoundData)}: {RoundData.Value}";
        }
    }
}