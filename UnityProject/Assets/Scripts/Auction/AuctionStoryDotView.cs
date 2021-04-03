using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class AuctionStoryDotView : ViewBase
    {
        [Inject] private AuctionSystem AuctionSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        
        private int _roughBet;
        
        public AuctionPlayerWidget WidgetPrefab;
        public RectTransform WidgetsRoot;

        public Text PlayerText;
        public Text BetText;
        
        public GameObject BetPanel;
        public Text RoughBetText;

        private AuctionData AuctionData => MatchData.QuestionAnswerData.AuctionData.Value;
        
        public void Initialize()
        {
            MatchData.QuestionAnswerData.AuctionData.SubscribeChanged(RefreshUI);
        }
        
        protected override void OnShown()
        {
            SetRoughBet(AuctionData.Bet);
            RefreshUI();
        }

        private void RefreshUI()
        {
            RefreshPlayersWidgets(MatchData.PlayersBoard.Value);
            BetPanel.SetActive(NetworkData.IsClient && !AuctionData.PassedPlayers.Contains(MatchData.ThisPlayer));
            PlayerText.text = AuctionData.Player?.Name ?? Static.EmptyPlayerName;
            BetText.text = AuctionData.Bet.ToString();
        }
        
        private void RefreshPlayersWidgets(PlayersBoard playersBoard)
        {
            ClearChild(WidgetsRoot);
            foreach (PlayerData playerData in playersBoard.Players)
            {
                AuctionPlayerWidget widget = Instantiate(WidgetPrefab, WidgetsRoot);
                widget.Bind(playerData, AuctionData);
            }
        }

        public void OnMakeBetButtonClicked()
        {
            AuctionSystem.SendPlayerBet(_roughBet);
        }

        public void OnAllInButtonClicked()
        {
            AuctionSystem.SendPlayerAllIn();
        }

        public void OnPassButtonClicked()
        {
            AuctionSystem.SendPlayerPass();
        }
        
        public void OnFinishAuctionButtonClicked()
        {
            AuctionSystem.MasterFinishAuction();
        }

        public void OnChangeBetButtonClicked(int changeValue)
        {
            SetRoughBet(_roughBet + changeValue);
        }

        private void SetRoughBet(int bet)
        {
            _roughBet = bet;
            RoughBetText.text = $"Поставить\n{_roughBet}";
        }
    }
}