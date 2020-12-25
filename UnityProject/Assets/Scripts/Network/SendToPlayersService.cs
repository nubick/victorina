using System.Collections.Generic;
using System.Linq;
using Injection;
using MLAPI;
using UnityEngine;

namespace Victorina
{
    public class SendToPlayersService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }

        private List<NetworkPlayer> GetPlayers()
        {
            return NetworkingManager.ConnectedClientsList.Where(_ => _.PlayerObject != null).Select(_ => _.PlayerObject.GetComponent<NetworkPlayer>()).ToList();
        }
        
        public void Send(PlayersBoard playersBoard)
        {
            Debug.Log("Admin. ServerService. Refresh");

            // PlayersBoard playersBoard = new PlayersBoard();
            // playersBoard.PlayerNames.Add("Name A");
            // playersBoard.PlayerNames.Add("Name B");
            // playersBoard.PlayerNames.Add("Name C");

            GetPlayers().ForEach(player => player.SendPlayersBoard(playersBoard));
        }

        public void Send(MatchData matchData)
        {
            Debug.Log($"Master: Send MatchData: {matchData}");
            GetPlayers().ForEach(player => player.SendMatchData(matchData));
        }

        public void Send(TextQuestion textQuestion)
        {
            Debug.Log($"Master: Send TextQuestion: {textQuestion}");
            GetPlayers().ForEach(player => player.SendTextQuestion(textQuestion));
        }
    }
}