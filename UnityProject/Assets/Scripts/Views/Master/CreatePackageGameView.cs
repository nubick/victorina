using System.Collections;
using Assets.Scripts.Utils;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class CreatePackageGameView : ViewBase
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private CreatePackageGameSystem CreatePackageGameSystem { get; set; }
        [Inject] private CreatePackageGameData CreatePackageGameData { get; set; }
        [Inject] private LobbyView LobbyView { get; set; }
        [Inject] private ConfirmationDialogueView ConfirmationDialogueView { get; set; }

        public GridLayoutGroup PackagesGrid;
        public RectTransform LoadedPackagesRoot;
        public LoadedPackageWidget LoadedPackageWidgetPrefab;
        public Color[] WidgetColors;

        public void Initialize()
        {
            MetagameEvents.LoadedPackageCreateGame.Subscribe(CreatePackageGame);
            MetagameEvents.LoadedPackageResumeGame.Subscribe(ResumePackageGame);
            MetagameEvents.LoadedPackageDelete.Subscribe(DeletePackage);
        }
        
        protected override void OnShown()
        {
            CreatePackageGameSystem.RefreshPackages();
            RefreshUI();
        }

        private void RefreshUI()
        {
            ClearChild(LoadedPackagesRoot);
            for (int i = 0; i < CreatePackageGameData.Packages.Count; i++)
            {
                LoadedPackageWidget widget = Instantiate(LoadedPackageWidgetPrefab, LoadedPackagesRoot);
                Color bgColor = WidgetColors[i % WidgetColors.Length];
                widget.Bind(CreatePackageGameData.Packages[i], bgColor);
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            RectTransform gridTransform = PackagesGrid.transform as RectTransform;
            float width = gridTransform.GetWidth();
            int columns = PackagesGrid.constraintCount;
            float widgetWidth = (width - PackagesGrid.padding.left - PackagesGrid.padding.right - (columns - 1) * PackagesGrid.spacing.x) / columns;
            PackagesGrid.cellSize = new Vector2(widgetWidth, PackagesGrid.cellSize.y);
        }

        public void OnLoadPackageButtonClicked()
        {
            CreatePackageGameSystem.LoadPackage();
            RefreshUI();
        }
        
        private void CreatePackageGame(Package package)
        {
            CreatePackageGameSystem.CreatePackageGame(package);
            SwitchTo(LobbyView);
        }

        private void ResumePackageGame(Package package)
        {
            CreatePackageGameSystem.ResumePackageGame(package);
            SwitchTo(LobbyView);
        }
        
        private void DeletePackage(Package package)
        {
            StartCoroutine(DeletePackageCoroutine(package));
        }

        private IEnumerator DeletePackageCoroutine(Package package)
        {
            yield return ConfirmationDialogueView.ShowAndWaitForFinish("Удаление пака", "Пак и вся связанная информация будут удалены.\nПродолжить?");
            if (ConfirmationDialogueView.IsConfirmed)
            {
                CreatePackageGameSystem.Delete(package);
                RefreshUI();
            }
        }
        
        public void OnBackButtonClicked()
        {
            SwitchTo(StartupView);
        }
    }
}