using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Injection;
using MLAPI;
using UnityEngine;

namespace Victorina
{
    public class SendToPlayersService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        
        private string All => $"All: ({GetPlayersInfo()})";
        
        private List<NetworkPlayer> GetPlayers()
        {
            return NetworkingManager.ConnectedClientsList.Where(_ => _.PlayerObject != null).Select(_ => _.PlayerObject.GetComponent<NetworkPlayer>()).ToList();
        }

        public void SendAll(NetworkPlayer networkPlayer)
        {
            networkPlayer.SendPlayersBoard(MatchData.PlayersBoard.Value);

            if (MatchData.Phase.Value == MatchPhase.WaitingInLobby)
            {

            }
            else if (MatchData.Phase.Value == MatchPhase.Round)
            {
                networkPlayer.SendNetRound(MatchData.RoundData.Value);
                networkPlayer.SendNetRoundsInfo(MatchData.RoundsInfo.Value);
                
                Round round = PackageData.Package.Rounds[MatchData.RoundsInfo.Value.CurrentRoundNumber - 1];
                (int[] fileIds, int[] chunksAmounts) info = PackageSystem.GetRoundFileIds(round);
                networkPlayer.SendRoundFileIds(info.fileIds, info.chunksAmounts);
                
            }
            else if (MatchData.Phase.Value == MatchPhase.Question)
            {
                networkPlayer.SendSelectedRoundQuestion(MatchData.SelectedRoundQuestion);
                networkPlayer.SendSelectedQuestion(QuestionAnswerData.SelectedQuestion.Value);
                networkPlayer.SendPlayersButtonClickData(QuestionAnswerData.PlayersButtonClickData.Value);
                networkPlayer.SendQuestionAnswerData(QuestionAnswerData);
            }
            
            networkPlayer.SendMatchPhase(MatchData.Phase.Value);
        }

        private string GetPlayersInfo()
        {
            return string.Join(",", GetPlayers().Select(_ => _.OwnerClientId.ToString()));
        }
        
        public void Send(PlayersBoard playersBoard)
        {
            Debug.Log($"Master: Send PlayersBoard to {All}: {playersBoard}");
            GetPlayers().ForEach(player => player.SendPlayersBoard(playersBoard));
        }

        public void SendMatchPhase(MatchPhase matchPhase)
        {
            Debug.Log($"Master: Send match phase to {All}: {matchPhase}");
            GetPlayers().ForEach(player => player.SendMatchPhase(matchPhase));
        }
        
        public void SendNetRound(NetRound netRound)
        {
            Debug.Log($"Master: Send NetRound '{netRound}' to {All}");
            GetPlayers().ForEach(player => player.SendNetRound(netRound));
        }

        public void SendSelectedQuestion(NetQuestion netQuestion)
        {
            Debug.Log($"Master: Send selected question to {All}: {netQuestion}");
            GetPlayers().ForEach(player => player.SendSelectedQuestion(netQuestion));
        }
        
        public void SendSelectedRoundQuestion(NetRoundQuestion netRoundQuestion)
        {
            Debug.Log($"Master: Send selected round question to {All} {netRoundQuestion}");
            GetPlayers().ForEach(player => player.SendSelectedRoundQuestion(netRoundQuestion));
        }
        
        public void SendNetRoundsInfo(NetRoundsInfo netRoundsInfo)
        {
            Debug.Log($"Master: Send rounds info to {All}, {netRoundsInfo}");
            GetPlayers().ForEach(player => player.SendNetRoundsInfo(netRoundsInfo));
        }

        public void SendRoundFileIds(int[] fileIds, int[] chunksAmounts)
        {
            Debug.Log($"Master: Send round file ids ({fileIds.Length}) to {All}");
            GetPlayers().ForEach(player => player.SendRoundFileIds(fileIds, chunksAmounts));
        }
        
        public void Send(QuestionAnswerData questionAnswerData)
        {
            Debug.Log($"Master: Send question answer data to {All}: {questionAnswerData}");
            GetPlayers().ForEach(player => player.SendQuestionAnswerData(questionAnswerData));
        }

        public void SendCatInBagData(CatInBagData data)
        {
            Debug.Log($"Master: Send cat in bag data to {All}: {data}");
            GetPlayers().ForEach(player => player.SendCatInBagData(data));
        }

        public void SendPlayersButtonClickData(PlayersButtonClickData playersButtonClickData)
        {
            Debug.Log($"Master: Send players button click data to {All}, ({playersButtonClickData.Players.Count})");
            GetPlayers().ForEach(player => player.SendPlayersButtonClickData(playersButtonClickData));
        }
        
        public void SendPlaySoundEffectCommand(int number)
        {
            Debug.Log($"Master: Send play sound effect command to {All}, number: {number}");
            GetPlayers().ForEach(player => player.SendPlaySoundEffectCommand(number));
        }
    }
}