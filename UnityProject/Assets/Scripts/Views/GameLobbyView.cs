using Injection;
using UnityEngine;

namespace Victorina
{
    public class GameLobbyView : ViewBase
    {
        [Inject] private MatchService MatchService { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private StartupView StartupView { get; set; }
        
        public PlayerWidget[] PlayerWidgets;
        public GameObject AdminPart;
        
        protected override void OnShown()
        {
            AdminPart.SetActive(NetworkData.IsAdmin);
            RefreshUI();
        }

        public void RefreshUI()
        {
            RefreshPlayersBoard(MatchData.PlayersBoard.Value);
        }

        private void RefreshPlayersBoard(PlayersBoard playersBoard)
        {
            for (int i = 0; i < PlayerWidgets.Length; i++)
            {
                PlayerWidgets[i].gameObject.SetActive(i < playersBoard.PlayerNames.Count);
                if (i < playersBoard.PlayerNames.Count)
                    PlayerWidgets[i].Bind(playersBoard.PlayerNames[i]);
            }
        }

        public void OnStartGameButtonClicked()
        {
            MatchService.Start();
        }

        public void OnBackButtonClicked()
        {
            ServerService.StopServer();
            SwitchTo(StartupView);
        }
    }
}