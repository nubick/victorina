using System.Collections;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class LobbyView : ViewBase
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private RoundView RoundView { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private SiqLoadedPackageSystem SiqLoadedPackageSystem { get; set; }
        [Inject] private SiqLoadedPackageData SiqLoadedPackageData { get; set; }
        [Inject] private ExternalIpData ExternalIpData { get; set; }
        [Inject] private IpCodeSystem IpCodeSystem { get; set; }
        [Inject] private PackageFilesSystem PackageFilesSystem { get; set; }

        public GameObject AdminPart;

        public GameObject[] OpenPackButtons;
        public Text[] OpenPackButtonTexts;
        public GameObject[] DeletePackButtons;

        public Text CodeText;
        
        protected override void OnShown()
        {
            AdminPart.SetActive(NetworkData.IsMaster);
            RefreshUI();
            StartCoroutine(RefreshCode());
        }

        protected override void OnHide()
        {
            StopAllCoroutines();
        }

        private IEnumerator RefreshCode()
        {
            if(!ExternalIpData.IsRequestFinished)
                CodeText.text = "Код Игры: вычисляю...";

            while (!ExternalIpData.IsRequestFinished)
                yield return null;

            if (ExternalIpData.HasError)
                CodeText.text = "Код Игры: ошибка!";
            else
                CodeText.text = $"Код Игры: {GetCode()}";
        }
        
        private string GetCode()
        {
            return IpCodeSystem.GetCode(ExternalIpData.Ip);
        }

        public void RefreshUI()
        {
            RefreshButtons();
        }
        
        public void OnBackButtonClicked()
        {
            ServerService.StopServer();
        }

        public void OnLoadPackButtonClicked()
        {
            string packageArchivePath = PackageFilesSystem.GetPackageArchivePathUsingOpenDialogue();
            
            if (string.IsNullOrWhiteSpace(packageArchivePath))
            {
                Debug.Log("Package archive is not selected in file browser.");
                return;
            }
            
            string packagePath = PackageFilesSystem.UnZipArchiveToPlayFolder(packageArchivePath);
            StartPackage(packagePath);
        }

        private void RefreshButtons()
        {
            SiqLoadedPackageSystem.Refresh();
            for (int i = 0; i < OpenPackButtons.Length; i++)
            {
                OpenPackButtons[i].SetActive(i < SiqLoadedPackageData.PackagesNames.Count);
                DeletePackButtons[i].SetActive(i < SiqLoadedPackageData.PackagesNames.Count);
                OpenPackButtonTexts[i].text = i < SiqLoadedPackageData.PackagesNames.Count ? SiqLoadedPackageData.PackagesNames[i] : string.Empty;
            }
        }

        public void OnOpenPackButtonClicked(int index)
        {
            string packagePath = SiqLoadedPackageData.PackagesPaths[index];
            StartPackage(packagePath);
        }

        private void StartPackage(string packagePath)
        {
            PackageSystem.Initialize(packagePath);
            MatchSystem.StartMatch();
            SwitchTo(RoundView);
        }

        public void OnDeletePackButtonClicked(int index)
        {
            string packagePath = SiqLoadedPackageData.PackagesPaths[index];
            PackageFilesSystem.Delete(packagePath);
            RefreshUI();
        }

        public void OnCopyCodeToClipboardButtonClicked()
        {
            if (ExternalIpData.IsRequestFinished && !ExternalIpData.HasError)
                GUIUtility.systemCopyBuffer = GetCode();
        }
    }
}