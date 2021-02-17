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
            List<NetworkPlayer> networkPlayers = GetPlayers();
            
            foreach (StoryDot storyDot in netQuestion.QuestionStory)
            {
                Debug.Log($"Master: Send question story dot to All: {storyDot}");
                networkPlayers.ForEach(player => player.SendStoryDot(storyDot, isQuestion: true));
            }

            foreach (StoryDot storyDot in netQuestion.AnswerStory)
            {
                Debug.Log($"Master: Send answer story dot to All: {storyDot}");
                networkPlayers.ForEach(player => player.SendStoryDot(storyDot, isQuestion: false));
            }
        }

        public void SendSelectedRoundQuestion(NetRoundQuestion netRoundQuestion)
        {
            Debug.Log($"Master: Send selected round question to All: {netRoundQuestion}");
            GetPlayers().ForEach(player => player.SendSelectedRoundQuestion(netRoundQuestion));
        }

        public void Send(QuestionAnswerData questionAnswerData)
        {
            Debug.Log($"Master: Send question answer data to All: {questionAnswerData}");
            GetPlayers().ForEach(player => player.SendQuestionAnswerData(questionAnswerData));
        }
        
        public void Send(NetRoundsInfo netRoundsInfo)
        {
            Debug.Log($"Master: Send rounds info to All: {netRoundsInfo}");
            GetPlayers().ForEach(player => player.SendNetRoundsInfo(netRoundsInfo));
        }

        public void SendRoundFileIds(int[] fileIds, int[] chunksAmounts)
        {
            Debug.Log($"Master: Send round file ids ({fileIds.Length}) to All");
            GetPlayers().ForEach(player => player.SendRoundFileIds(fileIds, chunksAmounts));
        }

        public void SendPlayersButtonClickData(PlayersButtonClickData playersButtonClickData)
        {
            Debug.Log($"Master: Send players button click data to All, ({playersButtonClickData.Players.Count})");
            GetPlayers().ForEach(player => player.SendPlayersButtonClickData(playersButtonClickData));
        }
    }
}