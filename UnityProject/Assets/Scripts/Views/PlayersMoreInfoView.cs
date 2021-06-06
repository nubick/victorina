using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayersMoreInfoView : ViewBase
    {
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayersMoreInfoData Data { get; set; }
        
        public RectTransform Root;
        public PlayerMoreInfoWidget WidgetPrefab;

        public void Initialize()
        {
            MetagameEvents.PlayersMoreInfoDataChanged.Subscribe(RefreshUI);
        }

        private void RefreshUI()
        {
            ClearChild(Root);
            for (int i = 0; i < Data.Size; i++)
            {
                PlayerMoreInfoWidget widget = Instantiate(WidgetPrefab, Root);
                widget.Bind(PlayersBoard.Players[i], Data.InfoTexts[i], Data.Highlights[i], Data.Selections[i]);
            }
        }
    }
}