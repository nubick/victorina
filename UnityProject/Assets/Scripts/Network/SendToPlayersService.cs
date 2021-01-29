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

        public void SendCurrentStoryDotIndex(int index)
        {
            Debug.Log($"Master: Send current story dot index to All: {index}");
            GetPlayers().ForEach(player => player.SendCurrentStoryDotIndex(index));
        }

        public void Send(NetRoundsInfo netRoundsInfo)
        {
            Debug.Log($"Master: Send rounds info to All: {netRoundsInfo}");
            GetPlayers().ForEach(player => player.SendNetRoundsInfo(netRoundsInfo));
        }

        public void SendStartTimer()
        {
            Debug.Log("Master: Send start timer to All");
            GetPlayers().ForEach(player => player.SendStartTimer());
        }

        public void SendStopTimer()
        {
            Debug.Log("Master: Send stop timer to All");
            GetPlayers().ForEach(player => player.SendStopTimer());
        }

        public void SendRoundFileIds(int[] fileIds, int[] chunksAmounts)
        {
            Debug.Log($"Master: Send round file ids ({fileIds.Length}) to All");
            GetPlayers().ForEach(player => player.SendRoundFileIds(fileIds, chunksAmounts));
        }
    }
}