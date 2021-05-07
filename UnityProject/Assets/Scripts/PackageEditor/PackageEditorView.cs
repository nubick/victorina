using Injection;
using UnityEngine;
using UnityEngine.Serialization;

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

        public RectTransform OpenedPackagesRoot;
        public OpenedPackageWidget OpenedPackageWidgetPrefab;

        public RectTransform RoundsRoot;
        public EditorRoundWidget EditorRoundWidgetPrefab;
        
        public void Initialize()
        {
            MetagameEvents.EditorPackageClicked.Subscribe(OnEditorPackageClicked);
            MetagameEvents.EditorRoundClicked.Subscribe(OnEditorRoundClicked);
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

            ClearChild(RoundsRoot);
            if (Data.SelectedPackage != null)
            {
                foreach (Round round in Data.SelectedPackage.Rounds)
                {
                    EditorRoundWidget roundWidget = Instantiate(EditorRoundWidgetPrefab, RoundsRoot);
                    roundWidget.Bind(round, round == Data.SelectedRound);
                }
            }
            
            
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

        private void OnEditorPackageClicked(string packageName)
        {
            Data.SelectedPackageName = packageName;
            Data.SelectedPackage = SiqConverter.Convert(packageName, PathData.PackageEditorPath);
            Data.SelectedRound = null;
            RefreshUI();
        }

        private void OnEditorRoundClicked(Round round)
        {
            Data.SelectedRound = round;
            RefreshUI();
        }
    }
}