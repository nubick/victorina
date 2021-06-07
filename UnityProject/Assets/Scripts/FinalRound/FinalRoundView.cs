using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class FinalRoundView : ViewBase
    {
        [Inject] private FinalRoundData FinalRoundData { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PlayersMoreInfoView PlayersMoreInfoView { get; set; }
        [Inject] private PlayersMoreInfoData PlayersMoreInfoData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        
        [Header("Phase 1: Phase Removing")]
        public GameObject Phase1State;
        public GameObject MasterPanelPhase1;
        public RectTransform ThemeButtonsRoot;
        public FinalRoundThemeButton RemoveThemeButtonPrefab;
        public Button AcceptBetsButton;

        [Header("Phase 2: Betting")]
        public GameObject Phase2State;
        public GameObject MasterPanelPhase2;
        public BetBoardWidget BetBoardWidget;
        public Text Theme;
        public GameObject MakeYourBetsLabel;
        public GameObject BetsDoneLabel;
        public GameObject YouPassLabel;
        
        public void Initialize()
        {
            MetagameEvents.FinalRoundThemeClicked.Subscribe(OnThemeClicked);
            MetagameEvents.FinalRoundDataChanged.Subscribe(OnFinalRoundDataChanged);
            MetagameEvents.PlayerMoreInfoClicked.Subscribe(OnPlayerMoreInfoClicked);

            BetBoardWidget.MakeBetEvent += OnPlayerMakeBet;
            BetBoardWidget.AllInEvent += OnPlayerAllIn;
        }
        
        protected override void OnShown()
        {
            RefreshUI();
        }

        private void OnFinalRoundDataChanged()
        {
            RefreshUI();
        }
        
        public void RefreshUI()
        {
            Phase1State.SetActive(FinalRoundData.Phase == FinalRoundPhase.ThemesRemoving);
            Phase2State.SetActive(FinalRoundData.Phase == FinalRoundPhase.Betting);
            
            UpdatePlayersMoreInfoView(FinalRoundData.Phase);
            
            if (FinalRoundData.Phase == FinalRoundPhase.ThemesRemoving)
            {
                RefreshThemesRemovingUI();
            }
            else if (FinalRoundData.Phase == FinalRoundPhase.Betting)
            {
                RefreshBettingUI();
            }
        }
        
        #region Themes Removing
        
        private void RefreshThemesRemovingUI()
        {
            MasterPanelPhase1.SetActive(NetworkData.IsMaster);
            AcceptBetsButton.interactable = FinalRoundSystem.IsRemovingFinished();

            ClearChild(ThemeButtonsRoot);
            for (int i = 0; i < FinalRoundData.Themes.Length; i++)
            {
                FinalRoundThemeButton button = Instantiate(RemoveThemeButtonPrefab, ThemeButtonsRoot);
                button.Bind(i, FinalRoundData.Themes[i], isEven: i % 2 == 0, isRemoved: FinalRoundData.RemovedThemes[i]);
            }
        }
        
        private void OnThemeClicked(int index)
        {
            FinalRoundSystem.TryRemoveTheme(index);
        }

        public void OnAcceptBetsButtonClicked()
        {
            FinalRoundSystem.ChangePhase(FinalRoundPhase.Betting);
        }

        public void OnPassFinalRoundButtonClicked()
        {
            
        }

        public void OnRestartButtonClicked()
        {
            FinalRoundSystem.Reset();
        }
        
        #endregion
        
        #region Phase 2: Betting
        
        private void RefreshBettingUI()
        {
            MasterPanelPhase2.SetActive(NetworkData.IsMaster);

            bool canThisParticipate = NetworkData.IsMaster || FinalRoundSystem.CanParticipate(MatchData.ThisPlayer);
            MakeYourBetsLabel.SetActive(canThisParticipate && !FinalRoundData.IsAllBetsDone);
            BetsDoneLabel.SetActive(canThisParticipate && FinalRoundData.IsAllBetsDone);
            YouPassLabel.SetActive(!canThisParticipate);

            Theme.text = $"Тема: {FinalRoundSystem.GetSelectedTheme()}";
            
            int size = PlayersBoard.Players.Count;
            string[] infoTexts = new string[size];
            bool[] highlights = new bool[size];
            bool[] selections = new bool[size];

            for (int i = 0; i < size; i++)
            {
                PlayerData player = PlayersBoard.Players[i];
                bool canParticipate = FinalRoundSystem.CanParticipate(player);
                
                if (NetworkData.IsClient)
                {
                    if (!canParticipate)
                        infoTexts[i] = "Пас";
                    else if (FinalRoundData.DoneBets[i])
                        infoTexts[i] = "Ставка сделана";
                    else
                        infoTexts[i] = "Делает ставку";
                }

                if (NetworkData.IsMaster)
                {
                    int bet = FinalRoundData.Bets[i];
                    if (!canParticipate)
                        infoTexts[i] = "Пас";
                    else if (bet > 0)
                        infoTexts[i] = bet.ToString();
                    else
                        infoTexts[i] = "Делает ставку";

                    selections[i] = PlayersBoard.Players[i] == FinalRoundData.SelectedPlayerByMaster;
                }

                highlights[i] = canParticipate && !FinalRoundData.DoneBets[i];
            }
            
            RefreshBetBoardWidget();
            
            PlayersMoreInfoData.Update(infoTexts, highlights, selections);
        }

        private void RefreshBetBoardWidget()
        {
            BetBoardWidget.gameObject.SetActive(false);

            if (NetworkData.IsClient && FinalRoundSystem.CanParticipate(MatchData.ThisPlayer))
            {
                BetBoardWidget.gameObject.SetActive(true);
                BindBetBoardWidget(MatchData.ThisPlayer);
            }

            if (NetworkData.IsMaster && FinalRoundData.SelectedPlayerByMaster != null)
            {
                BetBoardWidget.gameObject.SetActive(true);
                BindBetBoardWidget(FinalRoundData.SelectedPlayerByMaster);
            }
        }

        private void BindBetBoardWidget(PlayerData player)
        {
            int minBet = Mathf.Min(100, player.Score);
            int maxBet = player.Score;
            BetBoardWidget.Bind(minBet, maxBet, minBet);
            BetBoardWidget.SetSettings(true, true, true);
        }

        private void OnPlayerMakeBet(int bet)
        {
            FinalRoundSystem.TryMakeBet(bet);
        }

        private void OnPlayerAllIn()
        {
            FinalRoundSystem.TryMakeAllInBet();
        }

        public void OnShowQuestionButtonClicked()
        {
            FinalRoundSystem.ChangePhase(FinalRoundPhase.ThemesRemoving);
        }

        public void OnBackButtonClicked()
        {
            FinalRoundSystem.ChangePhase(FinalRoundPhase.ThemesRemoving);
        }
        
        #endregion
        
        private void UpdatePlayersMoreInfoView(FinalRoundPhase phase)
        {
            bool shouldBeVisible = phase == FinalRoundPhase.Betting;

            if (shouldBeVisible && !PlayersMoreInfoView.IsActive)
                PlayersMoreInfoView.Show();

            if (!shouldBeVisible && PlayersMoreInfoView.IsActive)
                PlayersMoreInfoView.Hide();
        }

        private void OnPlayerMoreInfoClicked(PlayerData player)
        {
            if (IsActive && NetworkData.IsMaster)
            {
                FinalRoundData.SelectPlayer(player);
                RefreshBetBoardWidget();
            }
        }
    }
}