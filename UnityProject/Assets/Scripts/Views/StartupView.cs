using Injection;

namespace Victorina
{
    public class StartupView : ViewBase
    {
        [Inject] private JoinGameView JoinGameView { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private GameLobbyView GameLobbyView { get; set; }
        [Inject] private RoundView RoundView { get; set; }
        
        public void OnCreateNewGameButtonClicked()
        {
            ServerService.StartServer();
            SwitchTo(GameLobbyView);
        }

        public void OnJoinGameButtonClicked()
        {
            SwitchTo(JoinGameView);
        }

        public void OnShowRoundButtonClicked()
        {
            SwitchTo(RoundView);
        }
    }
}