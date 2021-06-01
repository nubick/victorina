using Assets.Scripts.Data;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class AuctionView : ViewBase
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

        public SoundEffect AuctionStartSoundEffect;
        
        public Button FinishAuctionButton;
        
        private AuctionData AuctionData => MatchData.QuestionAnswerData.AuctionData.Value;
        
        public void Initialize()
        {
            MatchData.QuestionAnswerData.AuctionData.SubscribeChanged(RefreshUI);
            MetagameEvents.AuctionPlayerClicked.Subscribe(OnAuctionPlayerClicked);
        }
        
        protected override void OnShown()
        {
            AuctionStartSoundEffect.Play();
            RefreshUI();
        }

        private void RefreshUI()
        {
            RefreshPlayersWidgets(MatchData.PlayersBoard.Value);

            if (NetworkData.IsClient)
            {
                bool isBetPanelActive = !AuctionData.IsFinished && !AuctionData.PassedPlayers.Contains(MatchData.ThisPlayer);
                BetPanel.SetActive(isBetPanelActive);
                if (isBetPanelActive)
                {
                    BetCanvasGroup.interactable = AuctionData.BettingPlayer == MatchData.ThisPlayer && !AuctionData.IsAllIn;
                    AllInButton.interactable = AuctionData.BettingPlayer == MatchData.ThisPlayer && MatchData.ThisPlayer.Score >= AuctionData.NextMinBet;
                    PassButton.interactable = AuctionSystem.CanPass(MatchData.ThisPlayer);
                }
            }
            else if (NetworkData.IsMaster)
            {
                BetPanel.SetActive(AuctionData.SelectedPlayerByMaster != null);
                BetCanvasGroup.interactable = true;
                AllInButton.interactable = false;
                PassButton.interactable = false;
            }

            PlayerText.text = AuctionData.Player?.Name ?? Static.EmptyPlayerName;
            BetText.text = AuctionData.Bet.ToString();
            Theme.text = MatchData.SelectedRoundQuestion.Theme;
            
            FinishAuctionButton.gameObject.SetActive(NetworkData.IsMaster);
            FinishAuctionButton.interactable = AuctionData.Player != null;
            
            if (NetworkData.IsClient)
                SetRoughBet(AuctionData.NextMinBet);
        }
        
        private void RefreshPlayersWidgets(PlayersBoard playersBoard)
        {
            ClearChild(WidgetsRoot);
            foreach (PlayerData playerData in playersBoard.Players)
            {
                AuctionPlayerWidget widget = Instantiate(WidgetPrefab, WidgetsRoot);
                bool isSelected = NetworkData.IsMaster && AuctionData.SelectedPlayerByMaster == playerData;
                widget.Bind(playerData, AuctionData, isSelected);
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
            int minBet = NetworkData.IsClient ? AuctionData.NextMinBet : 0;
            int maxBet = NetworkData.IsClient ? Mathf.Max(MatchData.ThisPlayer.Score, AuctionData.NextMinBet) : int.MaxValue;
            _roughBet = Mathf.Clamp(bet, minBet, maxBet);
            RoughBetText.text = $"Поставить\n{_roughBet}";
        }
        
        private void OnAuctionPlayerClicked(PlayerData player)
        {
            if (NetworkData.IsMaster)
            {
                AuctionData.SelectedPlayerByMaster = player;
                RefreshUI();
            }
        }
    }
}