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
        public Text Theme;

        public GameObject BetPanel;
        public CanvasGroup BetCanvasGroup;
        public Button AllInButton;
        public Button PassButton;
        public Text RoughBetText;

        public GameObject FinishAuctionButton;
        
        private AuctionData AuctionData => MatchData.QuestionAnswerData.AuctionData.Value;
        
        public void Initialize()
        {
            MatchData.QuestionAnswerData.AuctionData.SubscribeChanged(RefreshUI);
        }
        
        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            RefreshPlayersWidgets(MatchData.PlayersBoard.Value);

            bool isBetPanelActive = NetworkData.IsClient && !AuctionData.IsFinished && !AuctionData.PassedPlayers.Contains(MatchData.ThisPlayer);
            BetPanel.SetActive(isBetPanelActive);
            if (isBetPanelActive)
            {
                BetCanvasGroup.interactable = AuctionData.BettingPlayer == MatchData.ThisPlayer && !AuctionData.IsAllIn;
                AllInButton.interactable = AuctionData.BettingPlayer == MatchData.ThisPlayer && MatchData.ThisPlayer.Score >= AuctionData.NextMinBet;
                PassButton.interactable = AuctionSystem.CanPass(MatchData.ThisPlayer);
            }

            PlayerText.text = AuctionData.Player?.Name ?? Static.EmptyPlayerName;
            BetText.text = AuctionData.Bet.ToString();
            Theme.text = MatchData.SelectedRoundQuestion.Theme;
            
            FinishAuctionButton.SetActive(NetworkData.IsMaster);
            
            if (NetworkData.IsClient)
                SetRoughBet(AuctionData.NextMinBet);
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
            _roughBet = Mathf.Clamp(bet, AuctionData.NextMinBet, Mathf.Max(MatchData.ThisPlayer.Score, AuctionData.NextMinBet));
            RoughBetText.text = $"Поставить\n{_roughBet}";
        }
    }
}