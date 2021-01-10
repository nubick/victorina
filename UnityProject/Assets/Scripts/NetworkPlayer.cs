using Injection;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Victorina
{
    public class NetworkPlayer : NetworkedBehaviour
    {
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

        public void SendMatchPhase(MatchPhase matchPhase)
        {
            Debug.Log($"Master: Send match phase: {matchPhase} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveMatchPhase, matchPhase);
        }

        [ClientRPC]
        private void ReceiveMatchPhase(MatchPhase matchPhase)
        {
            Debug.Log($"Player {OwnerClientId}: Receive match phase: {matchPhase}");
            MatchData.Phase.Value = matchPhase;
        }
        
        public void SendRoundData(NetRound netRound)
        {
            Debug.Log($"Master: Send RoundData: {netRound} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveRoundData, netRound, "RFS");
        }

        [ClientRPC]
        private void ReceiveRoundData(NetRound netRound)
        {
            Debug.Log($"Player {OwnerClientId}: Receive RoundData: {netRound}");
            MatchData.RoundData.Value = netRound;
        }
        
        public void SendSelectedQuestion(NetRoundQuestion netRoundQuestion)
        {
            Debug.Log($"Master: Send selected question: {netRoundQuestion} to {OwnerClientId}");
            InvokeClientRpcOnOwner(ReceiveSelectedQuestion, netRoundQuestion);
        }

        [ClientRPC]
        private void ReceiveSelectedQuestion(NetRoundQuestion netRoundQuestion)
        {
            Debug.Log($"Player {OwnerClientId}: Receive selected question: {netRoundQuestion}");
            MatchData.SelectedQuestion = netRoundQuestion;
        }
    }
}