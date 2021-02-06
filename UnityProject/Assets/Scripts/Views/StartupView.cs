using Injection;

namespace Victorina
{
    public class StartupView : ViewBase
    {
        [Inject] private JoinGameView JoinGameView { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private LobbyView LobbyView { get; set; }

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