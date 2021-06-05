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
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        
        public AuctionPlayerWidget WidgetPrefab;
        public RectTransform WidgetsRoot;

        public Text PlayerText;
        public Text BetText;
        public Text Theme;

        public BetBoardWidget BetBoardWidget;
        
        public SoundEffect AuctionStartSoundEffect;
        
        public Button FinishAuctionButton;
        
        private AuctionData AuctionData => MatchData.QuestionAnswerData.AuctionData.Value;
        
        public void Initialize()
        {
            MatchData.QuestionAnswerData.AuctionData.SubscribeChanged(RefreshUI);
            MetagameEvents.AuctionPlayerClicked.Subscribe(OnAuctionPlayerClicked);

            BetBoardWidget.MakeBetEvent += OnMakeBet;
            BetBoardWidget.AllInEvent += OnAllIn;
            BetBoardWidget.PassEvent += OnPass;
        }
        
        protected override void OnShown()
        {
            AuctionStartSoundEffect.Play();
            RefreshUI();
        }

        private void RefreshUI()
        {
            RefreshPlayersWidgets(PlayersBoard);

            if (NetworkData.IsClient)
            {
                bool isBetPanelActive = !AuctionData.IsFinished && !AuctionData.PassedPlayers.Contains(MatchData.ThisPlayer);
                BetBoardWidget.gameObject.SetActive(isBetPanelActive);
                if (isBetPanelActive)
                {
                    bool isInteractable = AuctionData.BettingPlayer == MatchData.ThisPlayer && !AuctionData.IsAllIn;
                    bool isAllInButtonActive = AuctionData.BettingPlayer == MatchData.ThisPlayer && MatchData.ThisPlayer.Score >= AuctionData.NextMinBet;
                    bool isPassButtonActive = AuctionSystem.CanPass(MatchData.ThisPlayer);
                    BetBoardWidget.SetSettings(isInteractable, isAllInButtonActive, isPassButtonActive);
                }
            }
            else if (NetworkData.IsMaster)
            {
                BetBoardWidget.gameObject.SetActive(AuctionData.SelectedPlayerByMaster != null);
                BetBoardWidget.SetSettings(isInteractable: true, isAllInButtonActive: false, isPassButtonActive: false);
            }

            PlayerText.text = AuctionData.Player?.Name ?? Static.EmptyPlayerName;
            BetText.text = AuctionData.Bet.ToString();
            Theme.text = MatchData.SelectedRoundQuestion.Theme;
            
            FinishAuctionButton.gameObject.SetActive(NetworkData.IsMaster);
            FinishAuctionButton.interactable = AuctionData.Player != null;
            
            int minBet = NetworkData.IsClient ? AuctionData.NextMinBet : 0;
            int maxBet = NetworkData.IsClient ? Mathf.Max(MatchData.ThisPlayer.Score, AuctionData.NextMinBet) : int.MaxValue;

            if (NetworkData.IsClient)
                BetBoardWidget.Bind(minBet, maxBet, AuctionData.NextMinBet);
            else if(NetworkData.IsMaster && AuctionData.SelectedPlayerByMaster != null)
                BetBoardWidget.Bind(minBet, maxBet, 100);
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

        private void OnMakeBet(int bet)
        {
            AuctionSystem.SendPlayerBet(bet);
        }

        private void OnAllIn()
        {
            AuctionSystem.SendPlayerAllIn();
        }

        private void OnPass()
        {
            AuctionSystem.SendPlayerPass();
        }
        
        public void OnFinishAuctionButtonClicked()
        {
            AuctionSystem.MasterFinishAuction();
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