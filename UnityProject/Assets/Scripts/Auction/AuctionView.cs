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
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        public Text PlayerText;
        public Text BetText;
        public Text Theme;

        public BetBoardWidget BetBoardWidget;
        
        public SoundEffect AuctionStartSoundEffect;
        
        public Button FinishAuctionButton;
        
        private AuctionPlayState AuctionPlayState => PlayStateData.As<AuctionPlayState>();
        
        public void Initialize()
        {
            MetagameEvents.PlayerMoreInfoClicked.Subscribe(OnPlayerMoreInfoClicked);

            BetBoardWidget.MakeBetEvent += OnMakeBet;
            BetBoardWidget.AllInEvent += OnAllIn;
            BetBoardWidget.PassEvent += OnPass;
        }
        
        protected override void OnShown()
        {
            //todo: finish refactoring
            //AuctionStartSoundEffect.Play();
            RefreshUI();
        }

        private void RefreshUI()
        {
            RefreshPlayersMoreInfoView(PlayersMoreInfoData, PlayersBoard);
            
            if (NetworkData.IsClient)
            {
                bool isBetPanelActive = !AuctionPlayState.IsFinished && !AuctionPlayState.PassedPlayers.Contains(MatchData.ThisPlayer);
                BetBoardWidget.gameObject.SetActive(isBetPanelActive);
                if (isBetPanelActive)
                {
                    bool isInteractable = AuctionPlayState.BettingPlayer == MatchData.ThisPlayer && !AuctionPlayState.IsAllIn;
                    bool isAllInButtonActive = AuctionPlayState.BettingPlayer == MatchData.ThisPlayer && MatchData.ThisPlayer.Score >= AuctionPlayState.NextMinBet;
                    bool isPassButtonActive = AuctionSystem.CanPass(MatchData.ThisPlayer);
                    BetBoardWidget.SetSettings(isInteractable, isAllInButtonActive, isPassButtonActive);
                }
            }
            else if (NetworkData.IsMaster)
            {
                BetBoardWidget.gameObject.SetActive(AuctionPlayState.SelectedPlayerByMaster != null);
                BetBoardWidget.SetSettings(isInteractable: true, isAllInButtonActive: false, isPassButtonActive: false);
            }

            PlayerText.text = AuctionPlayState.Player?.Name ?? Static.EmptyPlayerName;
            BetText.text = AuctionPlayState.Bet.ToString();
            Theme.text = AuctionPlayState.Theme;
            
            FinishAuctionButton.gameObject.SetActive(NetworkData.IsMaster);
            FinishAuctionButton.interactable = AuctionPlayState.Player != null;
            
            int minBet = NetworkData.IsClient ? AuctionPlayState.NextMinBet : 0;
            int maxBet = NetworkData.IsClient ? Mathf.Max(MatchData.ThisPlayer.Score, AuctionPlayState.NextMinBet) : int.MaxValue;

            if (NetworkData.IsClient)
                BetBoardWidget.Bind(minBet, maxBet, AuctionPlayState.NextMinBet);
            else if(NetworkData.IsMaster && AuctionPlayState.SelectedPlayerByMaster != null)
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
                infoTexts[i] = GetPlayerInfo(player, AuctionPlayState);
                highlights[i] = AuctionPlayState.BettingPlayer == player && AuctionPlayState.Player != player;
                selections[i] = NetworkData.IsMaster && AuctionPlayState.SelectedPlayerByMaster == player;
            }
            data.Update(infoTexts, highlights, selections);
        }

        private string GetPlayerInfo(PlayerData player, AuctionPlayState playState)
        {
            string infoText;
            if (playState.BettingPlayer == player)
                infoText = playState.Player == player ? "Выиграл" : "Делает ставку";
            else if (playState.Player == player)
                infoText = playState.IsAllIn ? "Ва-Банк" : playState.Bet.ToString();
            else
                infoText = playState.PassedPlayers.Contains(player) ? "Пас" : "Ожидание";
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
                AuctionPlayState.SelectedPlayerByMaster = player;
                RefreshUI();
            }
        }
    }
}