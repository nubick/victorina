using System.Collections;
using System.Collections.Generic;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PackageCrafterView : ViewBase
    {
        private readonly Dictionary<Theme, CrafterThemeLineWidget> _themeLineWidgetsCache = new Dictionary<Theme, CrafterThemeLineWidget>();
        private readonly Dictionary<Round, CrafterRoundTabWidget> _roundTabWidgetsCache = new Dictionary<Round, CrafterRoundTabWidget>();
        private readonly Dictionary<Question, CrafterQuestionWidget> _questionWidgetsCache = new Dictionary<Question, CrafterQuestionWidget>();
        
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private CrafterData Data { get; set; }
        [Inject] private PackageCrafterSystem PackageCrafterSystem { get; set; }
        [Inject] private ThemesSelectionFromBagView ThemesSelectionFromBagView { get; set; }
        [Inject] private InputDialogueView InputDialogueView { get; set; }
        [Inject] private CrafterQuestionEditView QuestionEditView { get; set; }

        public RectTransform OpenedPackagesRoot;
        public CrafterPackageTabWidget CrafterPackageTabWidgetPrefab;

        public RectTransform RoundsTabsRoot;
        public CrafterRoundTabWidget RoundTabWidgetPrefab;

        public RectTransform ThemesRoot;
        public CrafterThemeLineWidget ThemeLineWidgetPrefab;

        public CrafterQuestionWidget QuestionWidgetPrefab;
        public CrafterAddQuestionWidget AddQuestionWidgetPrefab;

        public RectTransform AddRoundPanel;
        public RectTransform AddThemePanel;

        [Header("Tools")]
        public GameObject SaveThemeButton;
        public GameObject SavePackageButton;

        public void Initialize()
        {
            MetagameEvents.CrafterPackageClicked.Subscribe(OnPackageClicked);
            
            MetagameEvents.CrafterQuestionClicked.Subscribe(OnQuestionClicked);
            MetagameEvents.CrafterQuestionDeleteButtonClicked.Subscribe(OnQuestionDeleteButtonClicked);
            MetagameEvents.CrafterPackageDeleteButtonClicked.Subscribe(OnPackageDeleteButtonClicked);
            MetagameEvents.CrafterQuestionEditRequested.Subscribe(OnQuestionEditRequested);

            MetagameEvents.CrafterRoundClicked.Subscribe(OnRoundClicked);
            MetagameEvents.CrafterRoundDeleteButtonClicked.Subscribe(OnRoundDeleteButtonClicked);
            MetagameEvents.CrafterRoundNameEditRequested.Subscribe(OnRoundNameEditRequested);
            
            MetagameEvents.CrafterThemeClicked.Subscribe(OnThemeClicked);
            MetagameEvents.CrafterThemeNameEditRequested.Subscribe(OnThemeNameEditRequested);
            MetagameEvents.CrafterThemeDeleteButtonClicked.Subscribe(OnThemeDeleteButtonClicked);
            MetagameEvents.CrafterThemeMoveToBagButtonClicked.Subscribe(OnThemeMoveToBagButtonClicked);
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

            _roundTabWidgetsCache.Clear();
            ClearChild(RoundsTabsRoot, AddRoundPanel.gameObject);
            RoundsTabsRoot.gameObject.SetActive(Data.SelectedPackage != null);
            if (Data.SelectedPackage != null)
            {
                foreach (Round round in Data.SelectedPackage.Rounds)
                {
                    CrafterRoundTabWidget roundTabWidget = Instantiate(RoundTabWidgetPrefab, RoundsTabsRoot);
                    roundTabWidget.Bind(round, round == Data.SelectedRound);
                    _roundTabWidgetsCache.Add(round, roundTabWidget);
                }
                AddRoundPanel.SetSiblingIndex(RoundsTabsRoot.childCount - 1);
            }
            
            RefreshThemes();
            
            SaveThemeButton.SetActive(Data.SelectedTheme != null);
            SavePackageButton.SetActive(Data.SelectedPackage != null);
        }

        private void RefreshThemes()
        {
            _themeLineWidgetsCache.Clear();
            _questionWidgetsCache.Clear();
            ClearChild(ThemesRoot, AddThemePanel.gameObject);
            ThemesRoot.gameObject.SetActive(Data.SelectedRound != null);
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
                        _questionWidgetsCache.Add(question, questionWidget);
                    }

                    CrafterAddQuestionWidget addQuestionWidget = Instantiate(AddQuestionWidgetPrefab, themeLineWidget.QuestionsRoot);
                    addQuestionWidget.Bind(theme);
                    
                    _themeLineWidgetsCache.Add(theme, themeLineWidget);
                }
                AddThemePanel.SetSiblingIndex(ThemesRoot.childCount - 1);
            }
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
            if (round == Data.SelectedRound)
                return;
            
            if (Data.SelectedRound != null)
                _roundTabWidgetsCache[Data.SelectedRound].UnSelect();
            
            PackageCrafterSystem.SelectRound(round);
            _roundTabWidgetsCache[round].Select();
            
            RefreshThemes();
        }
        
        private void OnThemeClicked(Theme theme)
        {
            if (Data.SelectedTheme != null)
                _themeLineWidgetsCache[Data.SelectedTheme].UnSelect();
            
            PackageCrafterSystem.SelectTheme(theme);
            _themeLineWidgetsCache[theme].Select();
        }

        private void OnQuestionClicked(Question question)
        {
            if (Data.SelectedQuestion != null)
                _questionWidgetsCache[Data.SelectedQuestion].UnSelect();
            
            PackageCrafterSystem.SelectQuestion(question);
            _questionWidgetsCache[Data.SelectedQuestion].Select();
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
            StartCoroutine(SelectFromBagCoroutine());
        }

        private IEnumerator SelectFromBagCoroutine()
        {
            yield return ThemesSelectionFromBagView.ShowAndWaitForFinish();
            RefreshUI();
        }
        
        private void OnThemeMoveToBagButtonClicked(Theme theme)
        {
            PackageCrafterSystem.MoveThemeToBag(theme);
            RefreshUI();
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
        
        private void OnThemeNameEditRequested(Theme theme)
        {
            StartCoroutine(ThemeNameEditCoroutine(theme));
        }

        private IEnumerator ThemeNameEditCoroutine(Theme theme)
        {
            yield return InputDialogueView.ShowAndWaitForFinish("Название темы" ,theme.Name);
            if (InputDialogueView.IsOk)
            {
                PackageCrafterSystem.ChangeName(theme, InputDialogueView.Text);
                RefreshUI();
            }
        }

        public void OnAddThemeButtonClicked()
        {
            Theme newTheme = PackageCrafterSystem.AddNewTheme();
            if (newTheme != null)
            {
                RefreshUI();
                OnThemeNameEditRequested(newTheme);
            }
        }

        private void OnRoundNameEditRequested(Round round)
        {
            StartCoroutine(RoundNameEditCoroutine(round));
        }

        private IEnumerator RoundNameEditCoroutine(Round round)
        {
            yield return InputDialogueView.ShowAndWaitForFinish("Название раунда", round.Name);
            if (InputDialogueView.IsOk)
            {
                PackageCrafterSystem.ChangeName(round, InputDialogueView.Text);
                RefreshUI();
            }
        }

        public void OnAddRoundButtonClicked()
        {
            PackageCrafterSystem.AddNewRound();
            RefreshUI();
        }
        
        private void OnQuestionEditRequested(Question question)
        {
            StartCoroutine(EditQuestionCoroutine());
        }

        private IEnumerator EditQuestionCoroutine()
        {
            yield return QuestionEditView.ShowAndWaitForFinish();
            RefreshUI();
        }
    }
}