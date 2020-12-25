using Injection;

namespace Victorina
{
    public class GameLobbySystem
    {
        [Inject] private GameLobbyView GameLobbyView { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
        public void Initialize()
        {
            MatchData.PlayersBoard.SubscribeChanged(() => GameLobbyView.RefreshUI());
        }
    }
}