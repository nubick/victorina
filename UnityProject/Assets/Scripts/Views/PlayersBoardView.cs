using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayersBoardView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MasterPlayerSettingsView MasterPlayerSettingsView { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }

        public PlayerBoardWidget WidgetPrefab;
        public RectTransform WidgetsRoot;
        
        public void Initialize()
        {
            MatchData.PlayersBoard.SubscribeChanged(RefreshUI);
            MetagameEvents.PlayerBoardWidgetClicked.Subscribe(OnPlayerBoardWidgetClicked);
        }

        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            ClearChild(WidgetsRoot);
            PlayersBoard playersBoard = MatchData.PlayersBoard.Value;
            foreach (PlayerData player in playersBoard.Players)
            {
                PlayerBoardWidget widget = Instantiate(WidgetPrefab, WidgetsRoot);
                widget.Bind(player, playersBoard.Current == player);
            }
        }

        private void OnPlayerBoardWidgetClicked(PlayerData playerData)
        {
            if (NetworkData.IsMaster)
            {
                MasterPlayerSettingsView.Bind(playerData);
                MasterPlayerSettingsView.Show();
            }
        }
    }
}