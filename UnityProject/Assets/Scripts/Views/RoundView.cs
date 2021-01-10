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

        public Transform ThemeWidgetsRoot;
        public ThemeWidget ThemeWidgetPrefab;

        public Transform QuestionsRoot;
        public Transform RoundQuestionsLinePrefab;
        public RoundQuestionWidget RoundQuestionWidgetPrefab;

        public void Initialize()
        {
            MetagameEvents.RoundQuestionClicked.Subscribe(OnRoundQuestionClicked);
        }
        
        protected override void OnShown()
        {
            RefreshUI(MatchData.RoundData.Value);
        }

        private void RefreshUI(NetRound netRound)
        {
            while (ThemeWidgetsRoot.childCount > 0)
                DestroyImmediate(ThemeWidgetsRoot.GetChild(0).gameObject);

            while (QuestionsRoot.childCount > 0)
                DestroyImmediate(QuestionsRoot.GetChild(0).gameObject);
            
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
        }
        
        public void OnBackButtonClicked()
        {
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
    }
}