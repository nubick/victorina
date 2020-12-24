using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class JoinGameView : ViewBase
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private ClientService ClientService { get; set; }
        [Inject] private GameLobbyView GameLobbyView { get; set; }

        public InputField PlayerNameInputField;
        public InputField IpInputField;
        
        public void OnJoinButtonClicked()
        {
            string playerName = PlayerNameInputField.text;
            string ip = IpInputField.text;
            ClientService.JoinGame(playerName, ip);
            //Is success - go to lobby
            SwitchTo(GameLobbyView);
        }

        public void OnBackButtonClicked()
        {
            SwitchTo(StartupView);
        }
    }
}