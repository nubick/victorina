using Injection;
using UnityEngine;

namespace Victorina
{
    public class PackageCrafterView : ViewBase
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private CrafterData Data { get; set; }
        [Inject] private PackageCrafterSystem PackageCrafterSystem { get; set; }
        [Inject] private ThemesSelectionFromBagView ThemesSelectionFromBagView { get; set; }

        public RectTransform OpenedPackagesRoot;
        public CrafterPackageTabWidget CrafterPackageTabWidgetPrefab;

        public RectTransform RoundsTabsRoot;
        public CrafterRoundTabWidget RoundTabWidgetPrefab;

        public RectTransform ThemesRoot;
        public CrafterThemeLineWidget ThemeLineWidgetPrefab;

        public CrafterQuestionWidget QuestionWidgetPrefab;
        public CrafterAddQuestionWidget AddQuestionWidgetPrefab;
        
        [Header("Tools")]
        public GameObject SaveThemeButton;
        public GameObject SavePackageButton;

        public void Initialize()
        {
            MetagameEvents.CrafterPackageClicked.Subscribe(OnPackageClicked);
            MetagameEvents.CrafterRoundClicked.Subscribe(OnRoundClicked);
            MetagameEvents.CrafterThemeClicked.Subscribe(OnThemeClicked);
            MetagameEvents.CrafterQuestionClicked.Subscribe(OnQuestionClicked);
            MetagameEvents.CrafterQuestionDeleteButtonClicked.Subscribe(OnQuestionDeleteButtonClicked);
            MetagameEvents.CrafterRoundDeleteButtonClicked.Subscribe(OnRoundDeleteButtonClicked);
            MetagameEvents.CrafterThemeDeleteButtonClicked.Subscribe(OnThemeDeleteButtonClicked);
            MetagameEvents.CrafterPackageDeleteButtonClicked.Subscribe(OnPackageDeleteButtonClicked);
        }
        
        protected override void OnShown()
        {
            PackageCrafterSystem.LoadPackages();
            RefreshUI();
        }

        public void RefreshUI()
        {
            ClearChild(OpenedPackagesRoot);
            foreach (Package package in Data.Packages)
            {
                CrafterPackageTabWidget tabWidget = Instantiate(CrafterPackageTabWidgetPrefab, OpenedPackagesRoot);
                tabWidget.Bind(package, isSelected: Data.SelectedPackage == package);
            }

            ClearChild(RoundsTabsRoot);
            if (Data.SelectedPackage != null)
            {
                foreach (Round round in Data.SelectedPackage.Rounds)
                {
                    CrafterRoundTabWidget roundTabWidget = Instantiate(RoundTabWidgetPrefab, RoundsTabsRoot);
                    roundTabWidget.Bind(round, round == Data.SelectedRound);
                }
            }
            
            ClearChild(ThemesRoot);
            if (Data.SelectedRound != null)
            {
                foreach (Theme theme in Data.SelectedRound.Themes)
                {
                    CrafterThemeLineWidget themeLineWidget = Instantiate(ThemeLineWidgetPrefab, ThemesRoot);
                    themeLineWidget.Bind(theme, Data.SelectedTheme == theme);

                    foreach (Question question in theme.Questions)
                    {
                        CrafterQuestionWidget questionWidget = Instantiate(QuestionWidgetPrefab, themeLineWidget.QuestionsRoot);
                        questionWidget.Bind(question, Data.SelectedQuestion == question);
                    }

                    CrafterAddQuestionWidget addQuestionWidget = Instantiate(AddQuestionWidgetPrefab, themeLineWidget.QuestionsRoot);
                    addQuestionWidget.Bind(theme);
                }
            }

            SaveThemeButton.SetActive(Data.SelectedTheme != null);
            SavePackageButton.SetActive(Data.SelectedPackage != null);
        }
        
        public void OnBackButtonClicked()
        {
            SwitchTo(StartupView);
        }

        public void OnAddPackButtonClicked()
        {
            PackageCrafterSystem.AddPackage();
            RefreshUI();
        }

        private void OnPackageClicked(Package package)
        {
            PackageCrafterSystem.SelectPackage(package);
            RefreshUI();
        }

        private void OnRoundClicked(Round round)
        {
            PackageCrafterSystem.SelectRound(round);
            RefreshUI();
        }
        
        private void OnThemeClicked(Theme theme)
        {
            PackageCrafterSystem.SelectTheme(theme);
            RefreshUI();
        }

        private void OnQuestionClicked(Question question)
        {
            PackageCrafterSystem.SelectQuestion(question);
            RefreshUI();
        }

        public void OnSaveThemeButtonClicked()
        {
            PackageCrafterSystem.SaveSelectedTheme();
        }

        public void OnSavePackageButtonClicked()
        {
            PackageCrafterSystem.SaveSelectedPackage();
        }

        public void OnPackageDeleteButtonClicked(Package package)
        {
            PackageCrafterSystem.DeletePackage(package);
            RefreshUI();
        }

        public void OnSelectBagButtonClicked()
        {
            ThemesSelectionFromBagView.Show();
        }

        public void OnCopyToBagButtonClicked()
        {
            PackageCrafterSystem.CopySelectedThemeToBag();
        }

        public void OnThemeDeleteButtonClicked(Theme theme)
        {
            PackageCrafterSystem.DeleteTheme(theme);
            RefreshUI();
        }

        public void OnQuestionDeleteButtonClicked(Question question)
        {
            PackageCrafterSystem.DeleteQuestion(question);
            RefreshUI();
        }

        public void OnRoundDeleteButtonClicked(Round round)
        {
            PackageCrafterSystem.DeleteRound(round);
            RefreshUI();
        }
    }
}