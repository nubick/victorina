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
            Debug.Log($"Master: Send PlayersBoard to All: {playersBoard}");
            GetPlayers().ForEach(player => player.SendPlayersBoard(playersBoard));
        }

        public void Send(MatchData matchData)
        {
            Debug.Log($"Master: Send MatchData to All: {matchData}");
            GetPlayers().ForEach(player => player.SendMatchData(matchData));
        }

        public void Send(TextQuestion textQuestion)
        {
            Debug.Log($"Master: Send TextQuestion to All: {textQuestion}");
            GetPlayers().ForEach(player => player.SendTextQuestion(textQuestion));
        }
    }
}