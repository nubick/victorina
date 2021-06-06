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
        [Inject] private PlayersMoreInfoData PlayersMoreInfoData { get; set; }
        
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
            MetagameEvents.PlayerMoreInfoClicked.Subscribe(OnPlayerMoreInfoClicked);

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
            RefreshPlayersMoreInfoView(PlayersMoreInfoData, PlayersBoard);
            
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

        private void RefreshPlayersMoreInfoView(PlayersMoreInfoData data, PlayersBoard playersBoard)
        {
            string[] infoTexts = new string[playersBoard.Players.Count];
            bool[] highlights = new bool[playersBoard.Players.Count];
            bool[] selections = new bool[playersBoard.Players.Count];
            for (int i = 0; i < playersBoard.Players.Count; i++)
            {
                PlayerData player = playersBoard.Players[i];
                infoTexts[i] = GetPlayerInfo(player, AuctionData);
                highlights[i] = AuctionData.BettingPlayer == player && AuctionData.Player != player;
                selections[i] = NetworkData.IsMaster && AuctionData.SelectedPlayerByMaster == player;
            }
            data.Update(infoTexts, highlights, selections);
        }

        private string GetPlayerInfo(PlayerData player, AuctionData auctionData)
        {
            string infoText;
            if (auctionData.BettingPlayer == player)
                infoText = auctionData.Player == player ? "Выиграл" : "Делает ставку";
            else if (auctionData.Player == player)
                infoText = auctionData.IsAllIn ? "Ва-Банк" : auctionData.Bet.ToString();
            else
                infoText = auctionData.PassedPlayers.Contains(player) ? "Пас" : "Ожидание";
            return infoText;
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
        
        private void OnPlayerMoreInfoClicked(PlayerData player)
        {
            if (IsActive && NetworkData.IsMaster)
            {
                AuctionData.SelectedPlayerByMaster = player;
                RefreshUI();
            }
        }
    }
}