using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class NoRiskStoryDotView : ViewBase
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }

        public Text PlayerName;
        
        protected override void OnShown()
        {
            PlayerName.text = PlayersBoardSystem.GetCurrentPlayerName();
        }
    }
}