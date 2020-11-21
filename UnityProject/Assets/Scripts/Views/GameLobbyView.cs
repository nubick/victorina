using Injection;

namespace Victorina
{
    public class GameLobbyView : ViewBase
    {
        [Inject] private GameLobbyData GameLobbyData { get; set; }
        
        public PlayerWidget[] PlayerWidgets;
        
        protected override void OnShown()
        {
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
    }
}