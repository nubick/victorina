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
        [Inject] private SiqPackageOpenSystem SiqPackageOpenSystem { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private SiqLoadedPackageSystem SiqLoadedPackageSystem { get; set; }
        [Inject] private SiqLoadedPackageData SiqLoadedPackageData { get; set; }
        [Inject] private ExternalIpData ExternalIpData { get; set; }
        [Inject] private IpCodeSystem IpCodeSystem { get; set; }

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
            string path = SiqPackageOpenSystem.GetPathUsingOpenDialogue();

            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.Log("Package is not selected in file browser.");
                return;
            }
            
            SiqPackageOpenSystem.UnZipPackageToPlayFolder(path);
            string packageName = SiqPackageOpenSystem.GetPackageName(path);
            StartPackage(packageName);
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
            MatchSystem.StartMatch();
            SwitchTo(RoundView);
        }

        public void OnDeletePackButtonClicked(int index)
        {
            string packageName = SiqLoadedPackageData.PackageNames[index];
            SiqLoadedPackageSystem.Delete(packageName);
            RefreshUI();
        }

        public void OnCopyCodeToClipboardButtonClicked()
        {
            if (ExternalIpData.IsRequestFinished && !ExternalIpData.HasError)
                GUIUtility.systemCopyBuffer = GetCode();
        }
    }
}