using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayersButtonClickPanelView : ViewBase, IDataDependOnlyView
    {
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        
        public RectTransform WidgetsRoot;
        public PlayerButtonClickWidget WidgetPrefab;
        
        public void Initialize()
        {
            QuestionAnswerData.PlayersButtonClickData.SubscribeChanged(Refresh);
            MetagameEvents.PlayerButtonClickWidgetClicked.Subscribe(OnWidgetClicked);
        }

        private void Refresh()
        {
            ClearChild(WidgetsRoot);
            Show();

            foreach (PlayerButtonClickData player in QuestionAnswerData.PlayersButtonClickData.Value.Players)
            {
                PlayerButtonClickWidget widget = Instantiate(WidgetPrefab, WidgetsRoot);
                widget.Bind(player);
            }
        }

        private void OnWidgetClicked(PlayerButtonClickData playerData)
        {
            QuestionAnswerSystem.SelectPlayerForAnswer(playerData.PlayerId);
        }
    }
}