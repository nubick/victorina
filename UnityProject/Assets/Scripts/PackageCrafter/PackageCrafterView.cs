using Injection;
using UnityEngine;
using UnityEngine.Serialization;

namespace Victorina
{
    public class PackageCrafterView : ViewBase
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private PackageCrafterData Data { get; set; }
        [Inject] private PackageCrafterSystem PackageCrafterSystem { get; set; }
        [Inject] private ThemesSelectionFromBagView ThemesSelectionFromBagView { get; set; }

        public RectTransform OpenedPackagesRoot;
        public OpenedPackageWidget OpenedPackageWidgetPrefab;

        public RectTransform RoundsTabsRoot;
        public PackageEditorRoundTabWidget RoundTabWidgetPrefab;

        public RectTransform ThemesRoot;
        public PackageEditorThemeWidget ThemeWidgetPrefab;

        public CrafterQuestionWidget QuestionWidgetPrefab;
        
        [Header("Tools")]
        public GameObject SaveThemeButton;
        public GameObject SavePackageButton;
        public GameObject DeletePackageButton;
        
        public void Initialize()
        {
            MetagameEvents.CrafterPackageClicked.Subscribe(OnPackageClicked);
            MetagameEvents.CrafterRoundClicked.Subscribe(OnRoundClicked);
            MetagameEvents.CrafterThemeClicked.Subscribe(OnThemeClicked);
            MetagameEvents.CrafterQuestionClicked.Subscribe(OnQuestionClicked);
        }
        
        protected override void OnShown()
        {
            PackageCrafterSystem.LoadPackages();
            RefreshUI();
        }

        private void RefreshUI()
        {
            ClearChild(OpenedPackagesRoot);
            foreach (Package package in Data.Packages)
            {
                OpenedPackageWidget widget = Instantiate(OpenedPackageWidgetPrefab, OpenedPackagesRoot);
                widget.Bind(package, isSelected: Data.SelectedPackage == package);
            }

            ClearChild(RoundsTabsRoot);
            if (Data.SelectedPackage != null)
            {
                foreach (Round round in Data.SelectedPackage.Rounds)
                {
                    PackageEditorRoundTabWidget roundTabWidget = Instantiate(RoundTabWidgetPrefab, RoundsTabsRoot);
                    roundTabWidget.Bind(round, round == Data.SelectedRound);
                }
            }
            
            ClearChild(ThemesRoot);
            if (Data.SelectedRound != null)
            {
                foreach (Theme theme in Data.SelectedRound.Themes)
                {
                    PackageEditorThemeWidget themeWidget = Instantiate(ThemeWidgetPrefab, ThemesRoot);
                    themeWidget.Bind(theme, Data.SelectedTheme == theme);

                    foreach (Question question in theme.Questions)
                    {
                        CrafterQuestionWidget questionWidget = Instantiate(QuestionWidgetPrefab, themeWidget.QuestionsRoot);
                        questionWidget.Bind(question, Data.SelectedQuestion == question);
                    }
                }
            }

            SaveThemeButton.SetActive(Data.SelectedTheme != null);
            SavePackageButton.SetActive(Data.SelectedPackage != null);
            DeletePackageButton.SetActive(Data.SelectedPackage != null);
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

        public void OnDeletePackageButtonClicked()
        {
            PackageCrafterSystem.DeleteSelectedPackage();
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

        public void OnDeleteThemeButtonClicked()
        {
            PackageCrafterSystem.DeleteSelectedTheme();
            RefreshUI();
        }

        public void OnDeleteQuestionButtonClicked()
        {
            PackageCrafterSystem.DeleteSelectedQuestion();
            RefreshUI();
        }

        public void OnDeleteRoundButtonClicked()
        {
            if (Data.SelectedRound == null)
                return;
            
            Data.SelectedPackage.Rounds.Remove(Data.SelectedRound);
            Data.SelectedRound = null;
            RefreshUI();
        }

        public void OnClearRoundButtonClicked()
        {
            if (Data.SelectedRound == null)
                return;

            Data.SelectedRound.Themes.Clear();
            RefreshUI();
        }
    }
}