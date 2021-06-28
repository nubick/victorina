using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class FinalRoundView : ViewBase
    {
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PlayersMoreInfoView PlayersMoreInfoView { get; set; }
        [Inject] private PlayersMoreInfoData PlayersMoreInfoData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }

        private FinalRoundPlayState PlayState => PlayStateData.As<FinalRoundPlayState>();
        
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

        [Header("Phase 4: Answering")]
        public GameObject Phase4State;
        public GameObject PlayerPanelPhase4;
        public GameObject MasterPanelPhase4;
        public InputField AnswerInputField;
        public Button SendAnswerButton;
        public GameObject AnswerPreviewPanel;
        public Text AnswerPreviewText;
        public GameObject SendYourAnswerLabel;
        public GameObject YouPassedLabelPhase4;
        public GameObject AnswerAcceptedLabel;

        [Header("Phase 5: Answers Accepting")]
        public GameObject Phase5State;
        public GameObject MasterPanelPhase5;
        public Text AnswerText;
        public Button ShowAnswerButton;
        public Button ShowBetButton;
        public Button WrongButton;
        public Button CorrectButton;
        public Button NextAcceptingPlayerButton;
        public Button FinishRoundButton;
        
        public void Initialize()
        {
            MetagameEvents.FinalRoundThemeClicked.Subscribe(OnThemeClicked);
            MetagameEvents.PlayerMoreInfoClicked.Subscribe(OnPlayerMoreInfoClicked);
            
            BetBoardWidget.MakeBetEvent += OnPlayerMakeBet;
            BetBoardWidget.AllInEvent += OnPlayerAllIn;
        }
        
        protected override void OnShown()
        {
            AnswerInputField.text = string.Empty;
            RefreshUI();
        }
        
        public void RefreshUI()
        {
            Phase1State.SetActive(PlayState.Phase == FinalRoundPhase.ThemesRemoving);
            Phase2State.SetActive(PlayState.Phase == FinalRoundPhase.Betting);
            Phase4State.SetActive(PlayState.Phase == FinalRoundPhase.Answering);
            Phase5State.SetActive(PlayState.Phase == FinalRoundPhase.AnswersAccepting);

            UpdatePlayersMoreInfoView(PlayState.Phase);

            switch (PlayState.Phase)
            {
                case FinalRoundPhase.ThemesRemoving:
                    RefreshThemesRemovingUI();
                    break;
                case FinalRoundPhase.Betting:
                    RefreshBettingUI();
                    break;
                case FinalRoundPhase.Answering:
                    RefreshAnsweringUI();
                    break;
                case FinalRoundPhase.AnswersAccepting:
                    RefreshAnswersAcceptingUI();
                    break;
            }
        }

        #region Phase 1: Themes Removing
        
        private void RefreshThemesRemovingUI()
        {
            MasterPanelPhase1.SetActive(NetworkData.IsMaster);
            AcceptBetsButton.interactable = PlayState.IsAllThemesRemoved;

            ClearChild(ThemeButtonsRoot);
            for (int i = 0; i < PlayState.Themes.Length; i++)
            {
                FinalRoundThemeButton button = Instantiate(RemoveThemeButtonPrefab, ThemeButtonsRoot);
                button.Bind(i, PlayState.Themes[i], isEven: i % 2 == 0, isRemoved: PlayState.RemovedThemes[i]);
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
            //todo: add and use PassFinalRoundCommand
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
            MakeYourBetsLabel.SetActive(canThisParticipate && !PlayState.IsAllBetsDone);
            BetsDoneLabel.SetActive(canThisParticipate && PlayState.IsAllBetsDone);
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
                    else if (PlayState.DoneBets[i])
                        infoTexts[i] = "Ставка сделана";
                    else
                        infoTexts[i] = "Делает ставку";
                }

                if (NetworkData.IsMaster)
                {
                    int bet = PlayState.Bets[i];
                    if (!canParticipate)
                        infoTexts[i] = "Пас";
                    else if (bet > 0)
                        infoTexts[i] = bet.ToString();
                    else
                        infoTexts[i] = "Делает ставку";

                    selections[i] = PlayersBoard.Players[i] == PlayState.SelectedPlayerByMaster;
                }

                highlights[i] = canParticipate && !PlayState.DoneBets[i];
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

            if (NetworkData.IsMaster && PlayState.SelectedPlayerByMaster != null)
            {
                BetBoardWidget.gameObject.SetActive(true);
                BindBetBoardWidget(PlayState.SelectedPlayerByMaster);
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
            FinalRoundSystem.ChangePhase(FinalRoundPhase.QuestionShowing);
        }

        public void OnBackButtonClicked()
        {
            FinalRoundSystem.ChangePhase(FinalRoundPhase.ThemesRemoving);
        }
        
        #endregion
        
        #region Phase 4: Answering

        public void RefreshAnsweringUI()
        {
            PlayerPanelPhase4.SetActive(NetworkData.IsClient);
            MasterPanelPhase4.SetActive(NetworkData.IsMaster);

            AnswerAcceptedLabel.SetActive(true);
            SendYourAnswerLabel.SetActive(true);
            YouPassedLabelPhase4.SetActive(false);
            
            int size = PlayersBoard.Players.Count;
            string[] infoTexts = new string[size];
            bool[] highlights = new bool[size];
            bool[] selections = new bool[size];

            for (int i = 0; i < size; i++)
            {
                PlayerData player = PlayersBoard.Players[i];
                bool canParticipate = FinalRoundSystem.CanParticipate(player);

                if (!canParticipate)
                    infoTexts[i] = "Пас";
                else if (PlayState.DoneAnswers[i])
                    infoTexts[i] = "Ответ принят";
                else
                    infoTexts[i] = "Отвечает";

                highlights[i] = canParticipate && !PlayState.DoneAnswers[i];

                if (NetworkData.IsMaster)
                    selections[i] = PlayersBoard.Players[i] == PlayState.SelectedPlayerByMaster;
                
                if (NetworkData.IsClient && MatchData.ThisPlayer == player)
                {
                    bool isAnswered = PlayState.DoneAnswers[i];
                    SendAnswerButton.interactable = !isAnswered;
                    AnswerInputField.interactable = !isAnswered;
                    AnswerAcceptedLabel.SetActive(canParticipate && isAnswered);
                    SendYourAnswerLabel.SetActive(canParticipate);
                    YouPassedLabelPhase4.SetActive(!canParticipate);
                }
            }
            
            AnswerPreviewPanel.SetActive(false);
            if (NetworkData.IsMaster && PlayState.SelectedPlayerByMaster != null)
            {
                AnswerPreviewPanel.SetActive(true);
                int selectedPlayerIndex = PlayersBoard.Players.IndexOf(PlayState.SelectedPlayerByMaster);
                AnswerPreviewText.text = PlayState.Answers[selectedPlayerIndex];
            }
            
            PlayersMoreInfoData.Update(infoTexts, highlights, selections);
        }
        
        public void OnSendAnswerButtonClicked()
        {
            FinalRoundSystem.SendAnswer(AnswerInputField.text);
        }

        public void OnClearAnswerButtonClicked()
        {
            FinalRoundSystem.ClearAnswer();
        }

        public void OnShowAnswersButtonClicked()
        {
            FinalRoundSystem.ChangePhase(FinalRoundPhase.AnswersAccepting);
        }
        
        #endregion
        
        #region Phase 5: Answers Accepting

        private void RefreshAnswersAcceptingUI()
        {
            MasterPanelPhase5.SetActive(NetworkData.IsMaster);
            AnswerText.text = PlayState.AcceptingInfo;
            ShowAnswerButton.interactable = PlayState.AcceptingPhase == FinalRoundAcceptingPhase.Name;
            CorrectButton.interactable = PlayState.AcceptingPhase == FinalRoundAcceptingPhase.Answer;
            WrongButton.interactable = PlayState.AcceptingPhase == FinalRoundAcceptingPhase.Answer;
            ShowBetButton.interactable = PlayState.AcceptingPhase == FinalRoundAcceptingPhase.Result;
            NextAcceptingPlayerButton.interactable = PlayState.AcceptingPhase == FinalRoundAcceptingPhase.Bet;
            FinishRoundButton.interactable = PlayState.AcceptingPhase == FinalRoundAcceptingPhase.Finish;
        }

        public void OnShowAcceptingAnswerButtonClicked()
        {
            FinalRoundSystem.ShowAcceptingAnswer();
        }
        
        public void OnCorrectButtonClicked()
        {
            FinalRoundSystem.AcceptAnswerAsCorrect();
        }

        public void OnWrongButtonClicked()
        {
            FinalRoundSystem.AcceptAnswerAsWrong();
        }

        public void OnShowAcceptingBetButtonClicked()
        {
            FinalRoundSystem.ApplyPlayerBet();
        }

        public void OnNextAcceptingPlayerButtonClicked()
        {
            FinalRoundSystem.SwitchToNextAcceptingPlayer();
        }

        public void OnFinishRoundButtonClicked()
        {
            FinalRoundSystem.FinishRound();
        }
        
        #endregion
        
        private void UpdatePlayersMoreInfoView(FinalRoundPhase phase)
        {
            bool shouldBeVisible = phase == FinalRoundPhase.Betting || phase == FinalRoundPhase.Answering;

            if (shouldBeVisible && !PlayersMoreInfoView.IsActive)
                PlayersMoreInfoView.Show();

            if (!shouldBeVisible && PlayersMoreInfoView.IsActive)
                PlayersMoreInfoView.Hide();
        }

        private void OnPlayerMoreInfoClicked(PlayerData player)
        {
            if (IsActive && NetworkData.IsMaster)
            {
                PlayState.SelectPlayer(player);
                RefreshBetBoardWidget();
            }
        }
    }
}