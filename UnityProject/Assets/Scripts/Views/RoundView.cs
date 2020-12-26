using Injection;
using UnityEngine;

namespace Victorina
{
    public class RoundView : ViewBase
    {
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private StartupView StartupView { get; set; }

        public Transform ThemeWidgetsRoot;
        public ThemeWidget ThemeWidgetPrefab;

        public Transform QuestionsRoot;
        public Transform RoundQuestionsLinePrefab;
        public RoundQuestionWidget RoundQuestionWidgetPrefab;
        
        protected override void OnShown()
        {
            int index = Random.Range(0, PackageData.Package.Rounds.Count);
            RefreshUI(PackageData.Package.Rounds[index]);
        }

        private void RefreshUI(Round round)
        {
            while (ThemeWidgetsRoot.childCount > 0)
                DestroyImmediate(ThemeWidgetsRoot.GetChild(0).gameObject);

            while (QuestionsRoot.childCount > 0)
                DestroyImmediate(QuestionsRoot.GetChild(0).gameObject);
            
            foreach (Theme theme in round.Themes)
            {
                ThemeWidget themeWidget = Instantiate(ThemeWidgetPrefab, ThemeWidgetsRoot);
                themeWidget.Name.text = theme.Name;

                Transform questionsRoot = Instantiate(RoundQuestionsLinePrefab, QuestionsRoot);
                foreach (Question question in theme.Questions)
                {
                    RoundQuestionWidget roundQuestionWidget = Instantiate(RoundQuestionWidgetPrefab, questionsRoot);
                    roundQuestionWidget.Price.text = question.Price.ToString();
                }
            }
        }

        public void OnBackButtonClicked()
        {
            SwitchTo(StartupView);
        }
    }
}