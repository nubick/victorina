using Injection;
using UnityEngine;

namespace Victorina
{
    public class ResultView : ViewBase
    {
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayStateSystem PlayStateSystem { get; set; }
        
        public GameObject LobbyButton;

        protected override void OnShown()
        {
            LobbyButton.SetActive(NetworkData.IsMaster);
        }

        public void OnLobbyButtonClicked()
        {
            PlayStateSystem.ChangePlayState(new LobbyPlayState());
        }
    }
}