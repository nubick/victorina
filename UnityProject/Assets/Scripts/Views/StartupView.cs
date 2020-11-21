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
            StartHost();
            SwitchTo(GameLobbyView);
        }

        public void OnJoinGameButtonClicked()
        {
            JoinGame();
        }

        private void StartHost()
        {
            ServerService.StartHost();
        }
        
        private void JoinGame()
        {
            SwitchTo(JoinGameView);
        }
    }
}