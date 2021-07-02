using System.Collections;
using Injection;
using UnityEngine;
using UnityEngine.UI;
using Victorina.Commands;

namespace Victorina
{
    public class LobbyView : ViewBase
    {
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private ClientService ClientService { get; set; }
        [Inject] private ExternalIpData ExternalIpData { get; set; }
        [Inject] private IpCodeSystem IpCodeSystem { get; set; }
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public GameObject MasterPart;
        public Text CodeText;
        
        protected override void OnShown()
        {
            MasterPart.SetActive(NetworkData.IsMaster);
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
        
        public void OnCopyCodeToClipboardButtonClicked()
        {
            if (ExternalIpData.IsRequestFinished && !ExternalIpData.HasError)
                GUIUtility.systemCopyBuffer = GetCode();
        }

        public void OnStartFirstRoundButtonClicked()
        {
            CommandsSystem.AddNewCommand(new SelectRoundCommand(1));
        }

        public void OnBackButtonClicked()
        {
            if (NetworkData.IsMaster)
                ServerService.StopServer();
            else
                ClientService.LeaveGame();

            SwitchTo(StartupView);
        }
    }
}