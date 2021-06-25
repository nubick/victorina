using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayersButtonClickPanelView : ViewBase, IDataDependOnlyView
    {
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private ShowQuestionSystem ShowQuestionSystem { get; set; }
        
        public RectTransform WidgetsRoot;
        public PlayerButtonClickWidget WidgetPrefab;

        public RectTransform VerticalLayoutGroup;
        public GameObject SelectFastestButton;
        
        public void Initialize()
        {
            MetagameEvents.PlayersButtonClickDataChanged.Subscribe(Refresh);
            MetagameEvents.PlayerButtonClickWidgetClicked.Subscribe(OnWidgetClicked);
        }

        private void Refresh()
        {
            ClearChild(WidgetsRoot);
            Show();

            foreach (PlayerButtonClickData player in PlayersButtonClickData.Players)
            {
                PlayerButtonClickWidget widget = Instantiate(WidgetPrefab, WidgetsRoot);
                widget.Bind(player);
            }

            SelectFastestButton.SetActive(NetworkData.IsMaster && PlayersButtonClickData.Players.Count > 1);
            LayoutRebuilder.ForceRebuildLayoutImmediate(VerticalLayoutGroup);
        }

        private void OnWidgetClicked(PlayerButtonClickData playerData)
        {
            ShowQuestionSystem.SelectPlayerForAnswer(playerData.PlayerId);
        }

        public void OnSelectFastestButtonClicked()
        {
            ShowQuestionSystem.SelectFastestPlayerForAnswer();   
        }
    }
}