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
        public Text HelpMessage;
        
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

            HelpMessage.text = GetHelpMessage(Static.Port);
        }

        private string GetHelpMessage(int port)
        {
            string helpMessageTemplate =
@"<b>Помощь:</b>
Для начала игры необходимо:
1. Иметь выделенный IP адрес
2. Октрыть порт <color=yellow>{0}</color> (TCP и UDP)
3. Скопировать код игры, кликнув по нему слева внизу экрана
4. Отправить код игры игрокам
5. Дождаться игроков
6. Начать первый раунд";
            return string.Format(helpMessageTemplate, port);
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