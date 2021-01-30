using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayersButtonClickPanelView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }

        public RectTransform WidgetsRoot;
        public PlayerButtonClickWidget WidgetPrefab;
        
        public void Initialize()
        {
            MatchData.QuestionAnsweringData.PlayersButtonClickData.SubscribeChanged(() => Refresh(MatchData.QuestionAnsweringData.PlayersButtonClickData.Value));
        }

        private void Refresh(PlayersButtonClickData data)
        {
            ClearChild(WidgetsRoot);
            Show();

            foreach (PlayerButtonClickData player in data.Players)
            {
                PlayerButtonClickWidget widget = Instantiate(WidgetPrefab, WidgetsRoot);
                widget.Name.text = player.Name;
                widget.Time.text = $"{player.Time:0.0} сек";
            }
        }
    }
}