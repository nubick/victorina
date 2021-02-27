using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class NoRiskStoryDotView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }
        
        public Text PlayerName;
        
        protected override void OnShown()
        {
            PlayerName.text = MatchData.PlayersBoard.Value.Current == null ? "Баба-Яга" : MatchData.PlayersBoard.Value.Current.Name;
        }
    }
}