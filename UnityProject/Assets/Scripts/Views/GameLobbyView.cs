using Injection;
using SFB;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class GameLobbyView : ViewBase
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private RoundView RoundView { get; set; }
        [Inject] private SiqPackOpenSystem SiqPackOpenSystem { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private SiqLoadedPackageSystem SiqLoadedPackageSystem { get; set; }
        [Inject] private SiqLoadedPackageData SiqLoadedPackageData { get; set; }
        
        public PlayerWidget[] PlayerWidgets;
        public GameObject AdminPart;

        public GameObject[] OpenPackButtons;
        public Text[] OpenPackButtonTexts;
        public GameObject[] DeletePackButtons;
        
        protected override void OnShown()
        {
            AdminPart.SetActive(NetworkData.IsAdmin);
            RefreshUI();
        }

        public void RefreshUI()
        {
            RefreshPlayersBoard(MatchData.PlayersBoard.Value);
            RefreshButtons();
        }

        private void RefreshPlayersBoard(PlayersBoard playersBoard)
        {
            for (int i = 0; i < PlayerWidgets.Length; i++)
            {
                PlayerWidgets[i].gameObject.SetActive(i < playersBoard.PlayerNames.Count);
                if (i < playersBoard.PlayerNames.Count)
                    PlayerWidgets[i].Bind(playersBoard.PlayerNames[i]);
            }
        }
        
        public void OnBackButtonClicked()
        {
            ServerService.StopServer();
            SwitchTo(StartupView);
        }

        public void OnLoadPackButtonClicked()
        {
            ExtensionFilter[] extensions = {new ExtensionFilter("SIQ Files", "siq")};
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            if (string.IsNullOrWhiteSpace(paths[0]))
            {
                Debug.Log("Not selected pack in file browser.");
            }
            else
            {
                string packageName = SiqPackOpenSystem.Open(paths[0]);
                StartPackage(packageName);
            }
        }

        private void RefreshButtons()
        {
            SiqLoadedPackageSystem.Refresh();
            for (int i = 0; i < OpenPackButtons.Length; i++)
            {
                OpenPackButtons[i].SetActive(i < SiqLoadedPackageData.PackageNames.Count);
                DeletePackButtons[i].SetActive(i < SiqLoadedPackageData.PackageNames.Count);
                OpenPackButtonTexts[i].text = i < SiqLoadedPackageData.PackageNames.Count ? SiqLoadedPackageData.PackageNames[i] : string.Empty;
            }
        }

        public void OnOpenPackButtonClicked(int index)
        {
            string packageName = SiqLoadedPackageData.PackageNames[index];
            StartPackage(packageName);
        }

        private void StartPackage(string packageName)
        {
            PackageSystem.Initialize(packageName);
            MatchSystem.Start();
            SwitchTo(RoundView);
        }

        public void OnDeletePackButtonClicked(int index)
        {
            string packageName = SiqLoadedPackageData.PackageNames[index];
            SiqLoadedPackageSystem.Delete(packageName);
            RefreshUI();
        }
    }
}