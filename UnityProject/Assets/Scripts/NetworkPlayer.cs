using Injection;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Victorina
{
    public class NetworkPlayer : NetworkedBehaviour
    {
        public string PlayerName;

        [Inject] private ViewsSystem ViewsSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
        public void Awake()
        {
            Debug.Log($"Network player created: {OwnerClientId}");
        }
        
        public void SetPlayerName(string playerName)
        {
            Debug.Log($"Client. Set player name: {playerName}");
            PlayerName = playerName;
            InvokeClientRpcOnEveryone(SetPlayerNameRPC, playerName);
        }
        
        [ClientRPC]
        public void SetPlayerNameRPC(string playerName)
        {
            Debug.Log($"Client RPC. Set player name: {playerName}");
            PlayerName = playerName;
        }

        public void SendPlayersBoard(PlayersBoard playersBoard)
        {
            Debug.Log($"Master: Send PlayersBoard: {playersBoard}");
            InvokeClientRpcOnOwner(UpdatePlayersBoardRPC, playersBoard);
        }
        
        [ClientRPC]
        private void UpdatePlayersBoardRPC(PlayersBoard playersBoard)
        {
            Debug.Log($"Player {OwnerClientId}: Wow, I am player and I got player board.");
            Debug.Log($"Players amount: {playersBoard.PlayerNames.Count}");
            //todo: update on player
        }

        public void SendMatchData(MatchData matchData)
        {
            Debug.Log($"Master: Send MatchData: {matchData}");
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
            Debug.Log($"Master: Send TextQuestion: {textQuestion}");
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