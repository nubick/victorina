using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class StartupView : ViewBase
    {
        [Inject] private JoinGameView JoinGameView { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private LobbyView LobbyView { get; set; }

        public Text Version;
        
        protected override void OnShown()
        {
            Version.text = $"Версия: {Static.DevSettings.GetVersion()}";
        }

        public void OnCreateNewGameButtonClicked()
        {
            ServerService.StartServer();
            SwitchTo(LobbyView);
        }

        public void OnJoinGameButtonClicked()
        {
            SwitchTo(JoinGameView);
        }
    }
}