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

        public void Send(MatchPhase matchPhase)
        {
            Debug.Log($"Master: Send match phase to All: {matchPhase}");
            GetPlayers().ForEach(player => player.SendMatchPhase(matchPhase));
        }
        
        public void Send(NetRound netRound)
        {
            Debug.Log($"Master: Send RoundData to All: {netRound}");
            GetPlayers().ForEach(player => player.SendRoundData(netRound));
        }

        public void SendSelectedQuestion(NetQuestion netQuestion)
        {
            Debug.Log($"Master: Send selected question to All: {netQuestion}");
            GetPlayers().ForEach(player => player.SendSelectedQuestion(netQuestion));
        }

        public void SendSelectedRoundQuestion(NetRoundQuestion netRoundQuestion)
        {
            Debug.Log($"Master: Send selected round question to All: {netRoundQuestion}");
            GetPlayers().ForEach(player => player.SendSelectedRoundQuestion(netRoundQuestion));
        }
    }
}