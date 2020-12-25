using Injection;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Victorina
{
    public class NetworkPlayer : NetworkedBehaviour
    {
        [Inject] private ViewsSystem ViewsSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
        public void Awake()
        {
            Debug.Log("Network player created");
        }
        
        public void SendPlayersBoard(PlayersBoard playersBoard)
        {
            Debug.Log($"Master: Send PlayersBoard: {playersBoard} to {OwnerClientId}");
            InvokeClientRpcOnOwner(UpdatePlayersBoardRPC, playersBoard);
        }
        
        [ClientRPC]
        private void UpdatePlayersBoardRPC(PlayersBoard playersBoard)
        {
            Debug.Log($"Player {OwnerClientId}: Receive PlayersBoard: {playersBoard}");
            MatchData.PlayersBoard.Value = playersBoard;
        }

        public void SendMatchData(MatchData matchData)
        {
            Debug.Log($"Master: Send MatchData: {matchData} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveMatchDataRPC, matchData);
        }

        [ClientRPC]
        private void ReceiveMatchDataRPC(MatchData matchData)
        {
            Debug.Log($"Player {OwnerClientId}: Receive matchData: {matchData}");
            MatchData.Phase = matchData.Phase;
            ViewsSystem.Refresh();
        }

        public void SendTextQuestion(TextQuestion textQuestion)
        {
            Debug.Log($"Master: Send TextQuestion: {textQuestion} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveTextQuestionRPC, textQuestion);
        }

        [ClientRPC]
        private void ReceiveTextQuestionRPC(TextQuestion textQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive TextQuestion: {textQuestion}");
            MatchData.TextQuestion = textQuestion;
            ViewsSystem.Refresh();
        }
    }
}