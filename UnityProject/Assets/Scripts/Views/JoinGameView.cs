using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class JoinGameView : ViewBase
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private ClientService ClientService { get; set; }

        public InputField PlayerNameInputField;
        
        public void OnJoinButtonClicked()
        {
            string playerName = PlayerNameInputField.text;
            ClientService.JoinGame(playerName);
        }

        public void OnBackButtonClicked()
        {
            SwitchTo(StartupView);
        }
    }
}