using Injection;
using UnityEngine;

namespace Victorina
{
    public class PackageEditorView : ViewBase
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private SiqPackageOpenSystem SiqPackageOpenSystem { get; set; }
        [Inject] private PackageEditorSystem System { get; set; }
        [Inject] private PackageEditorData Data { get; set; }
        [Inject] private SiqConverter SiqConverter { get; set; }
        [Inject] private PathData PathData { get; set; }
        [Inject] private PackageEditorSaveSystem SaveSystem { get; set; }

        public RectTransform OpenedPackagesRoot;
        public OpenedPackageWidget OpenedPackageWidgetPrefab;

        public RectTransform RoundsTabsRoot;
        public PackageEditorRoundTabWidget RoundTabWidgetPrefab;

        public RectTransform ThemesRoot;
        public PackageEditorThemeWidget ThemeWidget;

        [Header("Tools")]
        public GameObject SaveThemeButton;
        public GameObject SavePackageButton;
        
        public void Initialize()
        {
            MetagameEvents.EditorPackageClicked.Subscribe(OnPackageClicked);
            MetagameEvents.EditorRoundClicked.Subscribe(OnRoundClicked);
            MetagameEvents.EditorThemeClicked.Subscribe(OnThemeClicked);
        }
        
        protected override void OnShown()
        {
            Data.SelectedPackageName = null;
            Data.SelectedPackage = null;
            Data.SelectedRound = null;
            
            RefreshUI();
        }

        private void RefreshUI()
        {
            ClearChild(OpenedPackagesRoot);
            foreach (string packageName in System.GetOpenedPackagesNames())
            {
                OpenedPackageWidget widget = Instantiate(OpenedPackageWidgetPrefab, OpenedPackagesRoot);
                widget.Bind(packageName, isSelected: Data.SelectedPackageName == packageName);
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
                    PackageEditorThemeWidget themeWidget = Instantiate(ThemeWidget, ThemesRoot);
                    themeWidget.Bind(theme, Data.SelectedTheme == theme);
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
            string packagePath = SiqPackageOpenSystem.GetPathUsingOpenDialogue();

            if (string.IsNullOrEmpty(packagePath))
            {
                Debug.Log("Package is not selected in file browser.");
                return;
            }

            SiqPackageOpenSystem.UnZipPackageToEditorFolder(packagePath);

            RefreshUI();
        }

        private void OnPackageClicked(string packageName)
        {
            if (SiqConverter.IsValid(packageName, PathData.PackageEditorPath))
            {
                Data.SelectedPackage = SiqConverter.Convert(packageName, PathData.PackageEditorPath);
            }
            else
            {
                string path = $"{PathData.PackageEditorPath}/{packageName}/package.json";
                Data.SelectedPackage = System.LoadPackage(path);
            }
            
            Data.SelectedPackageName = packageName;
            Data.SelectedRound = null;
            Data.SelectedTheme = null;
            RefreshUI();
        }

        private void OnRoundClicked(Round round)
        {
            Data.SelectedRound = round;
            Data.SelectedTheme = null;
            RefreshUI();
        }
        
        private void OnThemeClicked(Theme theme)
        {
            Data.SelectedTheme = theme;
            RefreshUI();
        }

        public void OnSaveThemeButtonClicked()
        {
            if (Data.SelectedTheme != null)
            {
                SaveSystem.SaveTheme(Data.SelectedPackage, Data.SelectedTheme);
            }
        }

        public void OnSavePackageButtonClicked()
        {
            if (Data.SelectedPackage != null)
            {
                SaveSystem.SavePackage(Data.SelectedPackage);
            }
        }
    }
}