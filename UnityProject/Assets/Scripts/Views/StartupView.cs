using Injection;

namespace Victorina
{
    public class StartupView : ViewBase
    {
        [Inject] private JoinGameView JoinGameView { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private GameLobbyView GameLobbyView { get; set; }
        
        public void OnCreateNewGameButtonClicked()
        {
            ServerService.StartHost();
            SwitchTo(GameLobbyView);
        }

        public void OnJoinGameButtonClicked()
        {
            SwitchTo(JoinGameView);
        }
    }
}