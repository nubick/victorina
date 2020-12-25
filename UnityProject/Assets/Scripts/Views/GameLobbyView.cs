using Injection;
using UnityEngine;

namespace Victorina
{
    public class GameLobbyView : ViewBase
    {
        [Inject] private GameLobbyData GameLobbyData { get; set; }
        [Inject] private MatchService MatchService { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        
        public PlayerWidget[] PlayerWidgets;
        public GameObject AdminPart;
        
        protected override void OnShown()
        {
            AdminPart.SetActive(NetworkData.IsAdmin);
            RefreshUI();
        }

        public void RefreshUI()
        {
            for (int i = 0; i < PlayerWidgets.Length; i++)
            {
                PlayerWidgets[i].gameObject.SetActive(i < GameLobbyData.NetworkPlayers.Count);
                if (i < GameLobbyData.NetworkPlayers.Count)
                    PlayerWidgets[i].Bind(GameLobbyData.NetworkPlayers[i]);
            }
        }

        public void OnStartGameButtonClicked()
        {
            MatchService.Start();
        }
    }
}