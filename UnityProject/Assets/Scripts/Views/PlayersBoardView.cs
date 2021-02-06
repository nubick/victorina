using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayersBoardView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }

        public PlayerBoardWidget WidgetPrefab;
        public RectTransform WidgetsRoot;
        
        public void Initialize()
        {
            MatchData.PlayersBoard.SubscribeChanged(RefreshUI);
        }

        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            ClearChild(WidgetsRoot);
            foreach (PlayerData player in MatchData.PlayersBoard.Value.Players)
            {
                PlayerBoardWidget widget = Instantiate(WidgetPrefab, WidgetsRoot);
                widget.Bind(player.Name, player.Score);
            }
        }
    }
}