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
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PackagePlayStateData PackagePlayStateData { get; set; }
        
        public PlayerBoardWidget WidgetPrefab;
        public RectTransform WidgetsRoot;
        
        public void Initialize()
        {
            MetagameEvents.PlayersBoardChanged.Subscribe(RefreshUI);
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
            while (_widgets.Count < PlayersBoard.Players.Count)
            {
                PlayerBoardWidget widget = Instantiate(WidgetPrefab, WidgetsRoot);
                _widgets.Add(widget);
            }

            while (_widgets.Count > PlayersBoard.Players.Count)
            {
                PlayerBoardWidget widget = _widgets.Last();
                _widgets.Remove(widget);
                Destroy(widget.gameObject);
            }

            for (int i = 0; i < PlayersBoard.Players.Count; i++)
            {
                PlayerBoardWidget widget = _widgets[i];
                PlayerData player = PlayersBoard.Players[i];
                widget.Bind(player, PlayersBoard.Current == player);
            }
        }

        private void OnPlayerBoardWidgetClicked(PlayerData playerData)
        {
            if (NetworkData.IsMaster &&
                (PackagePlayStateData.Type == PlayStateType.Lobby || PackagePlayStateData.Type == PlayStateType.Round))
            {
                MasterPlayerSettingsView.Show(playerData);
            }
        }
    }
}