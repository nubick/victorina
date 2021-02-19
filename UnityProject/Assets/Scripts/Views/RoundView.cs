using System.Collections;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class RoundView : ViewBase
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private PlayersBoardView PlayersBoardView { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        
        public RectTransform ThemeWidgetsRoot;
        public ThemeWidget ThemeWidgetPrefab;

        public RectTransform QuestionsRoot;
        public Transform RoundQuestionsLinePrefab;
        public RoundQuestionWidget RoundQuestionWidgetPrefab;

        [Header("Rounds Info")] 
        public RectTransform RoundsInfoRoot;
        public RoundInfoWidget RoundInfoWidgetPrefab;
        
        public void Initialize()
        {
            MetagameEvents.RoundQuestionClicked.Subscribe(OnRoundQuestionClicked);
            MetagameEvents.RoundInfoClicked.Subscribe(OnRoundInfoClicked);
        }
        
        protected override void OnShown()
        {
            PlayersBoardView.Show();
            RefreshUI(MatchData.RoundData.Value);
        }

        protected override void OnHide()
        {
            PlayersBoardView.Hide();
        }

        private void RefreshUI(NetRound netRound)
        {
            ClearChild(ThemeWidgetsRoot);
            ClearChild(QuestionsRoot);
            
            int columns = netRound.Themes.Max(_ => _.Questions.Count);
            
            foreach (NetRoundTheme netRoundTheme in netRound.Themes)
            {
                ThemeWidget themeWidget = Instantiate(ThemeWidgetPrefab, ThemeWidgetsRoot);
                themeWidget.Name.text = netRoundTheme.Name;
                
                Transform questionsRoot = Instantiate(RoundQuestionsLinePrefab, QuestionsRoot);
                foreach (NetRoundQuestion netRoundQuestion in netRoundTheme.Questions)
                {
                    RoundQuestionWidget widget = Instantiate(RoundQuestionWidgetPrefab, questionsRoot);
                    widget.Bind(netRoundQuestion);
                }

                for (int i = netRoundTheme.Questions.Count; i < columns; i++)
                {
                    RoundQuestionWidget widget = Instantiate(RoundQuestionWidgetPrefab, questionsRoot);
                    widget.BindEmpty();
                }
            }
            
            RefreshRoundsInfo(MatchData.RoundsInfo.Value);
        }

        private void RefreshRoundsInfo(NetRoundsInfo roundsInfo)
        {
            ClearChild(RoundsInfoRoot);
            for (int number = 1; number <= roundsInfo.RoundsAmount; number++)
            {
                RoundInfoWidget widget = Instantiate(RoundInfoWidgetPrefab, RoundsInfoRoot);
                RoundProgress roundProgress =
                    number < roundsInfo.CurrentRoundNumber ? RoundProgress.Passed :
                    number > roundsInfo.CurrentRoundNumber ? RoundProgress.Next : RoundProgress.Current;
                widget.Bind($"Раунд {number}", number, roundProgress); //passed, current, next
            }
        }

        public void OnBackButtonClicked()
        {
            ServerService.StopServer();
            SwitchTo(StartupView);
        }
        
        private void OnRoundQuestionClicked(NetRoundQuestion netRoundQuestion)
        {
            MatchSystem.TrySelectQuestion(netRoundQuestion);
        }

        public IEnumerator ShowQuestionBlinkEffect(NetRoundQuestion netRoundQuestion)
        {
            RoundQuestionWidget widget = QuestionsRoot.GetComponentsInChildren<RoundQuestionWidget>().Single(_ => _.NetRoundQuestion != null && _.NetRoundQuestion.QuestionId == netRoundQuestion.QuestionId);
            for (int i = 0; i < 5; i++)
            {
                widget.ShowHighlighted();
                yield return new WaitForSeconds(0.1f);
                widget.ShowDefault();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void OnRoundInfoClicked(int number)
        {
            MatchSystem.SelectRound(number);
        }
    }
}