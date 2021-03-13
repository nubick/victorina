using System.Collections.Generic;
using System.Linq;
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
            _widgets.Clear();
            ClearChild(WidgetsRoot);
            RefreshUI();
        }

        private readonly List<PlayerBoardWidget> _widgets = new List<PlayerBoardWidget>();
        
        private void RefreshUI()
        {
            PlayersBoard playersBoard = MatchData.PlayersBoard.Value;

            while (_widgets.Count < playersBoard.Players.Count)
            {
                PlayerBoardWidget widget = Instantiate(WidgetPrefab, WidgetsRoot);
                _widgets.Add(widget);
            }

            while (_widgets.Count > playersBoard.Players.Count)
            {
                PlayerBoardWidget widget = _widgets.Last();
                _widgets.Remove(widget);
                Destroy(widget.gameObject);
            }

            for (int i = 0; i < playersBoard.Players.Count; i++)
            {
                PlayerBoardWidget widget = _widgets[i];
                PlayerData player = playersBoard.Players[i];
                widget.Bind(player, playersBoard.Current == player);
            }
        }

        private void OnPlayerBoardWidgetClicked(PlayerData playerData)
        {
            if (NetworkData.IsMaster && MatchData.Phase.Value != MatchPhase.Question)
            {
                MasterPlayerSettingsView.Show(playerData);
            }
        }
    }
}