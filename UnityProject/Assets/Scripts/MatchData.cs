using System;

namespace Victorina
{
    public class MatchData
    {
        public ReactiveProperty<MatchPhase> Phase { get; set; } = new ReactiveProperty<MatchPhase>();
        public ReactiveProperty<PlayersBoard> PlayersBoard { get; } = new ReactiveProperty<PlayersBoard>();
        public ReactiveProperty<NetRoundsInfo> RoundsInfo { get; } = new ReactiveProperty<NetRoundsInfo>();
        public ReactiveProperty<NetRound> RoundData { get; } = new ReactiveProperty<NetRound>();
        public NetRoundQuestion SelectedRoundQuestion { get; set; }

        public ReactiveProperty<NetQuestion> SelectedQuestion { get; } = new ReactiveProperty<NetQuestion>();
        public ReactiveProperty<int> CurrentStoryDotIndex { get; } = new ReactiveProperty<int>();

        public StoryDot CurrentStoryDot
        {
            get
            {
                if (Phase.Value == MatchPhase.ShowQuestion)
                {
                    return SelectedQuestion.Value.QuestionStory[CurrentStoryDotIndex.Value];    
                }
                else if (Phase.Value == MatchPhase.ShowAnswer)
                {
                    return SelectedQuestion.Value.AnswerStory[CurrentStoryDotIndex.Value];
                }
                else
                {
                    throw new Exception($"Can't get CurrentStoryDot for phase: {Phase.Value}");
                }
            }
        }

        public ReactiveProperty<PlayersButtonClickData> PlayersButtonClickData { get; } = new ReactiveProperty<PlayersButtonClickData>();
        
        public bool WasTimerStarted { get; set; }
        public bool IsTimerOn { get; set; }

        //Player only data
        public PlayerMatchData Player { get; set; }
        
        public MatchData()
        {
            PlayersBoard.Value = new PlayersBoard();
            RoundData.Value = new NetRound();
            RoundsInfo.Value = new NetRoundsInfo();
            PlayersButtonClickData.Value = new PlayersButtonClickData();
            Player = new PlayerMatchData();
        }
        
        public override string ToString()
        {
            return $"{nameof(Phase)}: {Phase.Value}, {nameof(PlayersBoard)}: {PlayersBoard.Value}, {nameof(RoundData)}: {RoundData.Value}";
        }
    }
}