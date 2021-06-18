using System;
using System.Collections.Generic;
using System.Linq;
using Injection;
using MLAPI;
using MLAPI.Connection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class SendToPlayersService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        
        private string All => $"All: ({GetPlayersInfo()})";
        
        private List<NetworkPlayer> GetPlayers()
        {
            return NetworkingManager.ConnectedClientsList.Where(_ => _.PlayerObject != null).Select(_ => _.PlayerObject.GetComponent<NetworkPlayer>()).ToList();
        }

        private NetworkPlayer GetPlayer(PlayerData player)
        {
            ulong clientId = ConnectedPlayersData.GetClientId(player.PlayerId);
            NetworkedClient networkedClient = NetworkingManager.ConnectedClientsList.SingleOrDefault(_ => _.ClientId == clientId);
            if (networkedClient == null)
                throw new Exception($"Can't find networkedClient for player {player}");
            return networkedClient.PlayerObject.GetComponent<NetworkPlayer>();
        }

        public void SendConnectionData(NetworkPlayer networkPlayer, byte registeredPlayerId)
        {
            networkPlayer.SendRegisteredPlayerId(registeredPlayerId);
            
            Debug.Log($"Master: Send PlayersBoard: {PlayersBoard} to {networkPlayer.GetPlayerInfo()}");
            networkPlayer.SendPlayersBoard(PlayersBoard);

            if (MatchData.Phase.Value == MatchPhase.WaitingInLobby)
            {

            }
            else if (MatchData.Phase.Value == MatchPhase.Round)
            {
                Debug.Log($"Master: Send RoundData: {MatchData.RoundData.Value} to {networkPlayer.GetPlayerInfo()}");
                networkPlayer.SendNetRound(MatchData.RoundData.Value);
                
                Debug.Log($"Master: Send RoundsInfo: {MatchData.RoundsInfo.Value} to {networkPlayer.GetPlayerInfo()}");
                networkPlayer.SendNetRoundsInfo(MatchData.RoundsInfo.Value);
            }
            else if (MatchData.Phase.Value == MatchPhase.Question)
            {
                networkPlayer.SendSelectedRoundQuestion(MatchData.SelectedRoundQuestion);
                networkPlayer.SendSelectedQuestion(QuestionAnswerData.SelectedQuestion.Value);
                networkPlayer.SendPlayersButtonClickData(QuestionAnswerData.PlayersButtonClickData);
                networkPlayer.SendQuestionAnswerData(QuestionAnswerData);
            }
            
            (int[] fileIds, int[] chunksAmounts, int[] priorities) info = PackageSystem.GetPackageFilesInfo(PackageData.Package);
            networkPlayer.SendRoundFileIds(info.fileIds, info.chunksAmounts, info.priorities);
            
            Debug.Log($"Master: Send match phase: {MatchData.Phase.Value} to {networkPlayer.GetPlayerInfo()}");
            networkPlayer.SendMatchPhase(MatchData.Phase.Value);
        }

        private string GetPlayersInfo()
        {
            return string.Join(",", GetPlayers().Select(_ => _.OwnerClientId.ToString()));
        }
        
        public void Send(PlayersBoard playersBoard)
        {
            //Debug.Log($"Master: Send PlayersBoard to {All}: {playersBoard}");
            GetPlayers().ForEach(player => player.SendPlayersBoard(playersBoard));
        }

        public void SendMatchPhase(MatchPhase matchPhase)
        {
            Debug.Log($"Master: Send match phase to {All}: {matchPhase}");
            GetPlayers().ForEach(player => player.SendMatchPhase(matchPhase));
        }
        
        public void SendNetRound(NetRound netRound)
        {
            //Debug.Log($"Master: Send NetRound '{netRound}' to {All}");
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
        
        public void Send(QuestionAnswerData questionAnswerData)
        {
            Debug.Log($"Master: Send QuestionAnswerData to {All}: {questionAnswerData}");
            GetPlayers().ForEach(player => player.SendQuestionAnswerData(questionAnswerData));
        }

        public void Send(QuestionStoryShowData data)
        {
            Debug.Log($"Master: Send QuestionStoryShowData to {All}: {data}");
            GetPlayers().ForEach(player => player.SendQuestionStoryShowData(data));
        }
        
        public void SendCatInBagData(CatInBagData data)
        {
            Debug.Log($"Master: Send cat in bag data to {All}: {data}");
            GetPlayers().ForEach(player => player.SendCatInBagData(data));
        }

        public void SendPlayersButtonClickData(PlayersButtonClickData playersButtonClickData)
        {
            Debug.Log($"Master: Send players button click data to {All}, {playersButtonClickData})");
            GetPlayers().ForEach(player => player.SendPlayersButtonClickData(playersButtonClickData));
        }
        
        public void SendPlaySoundEffectCommand(int number)
        {
            Debug.Log($"Master: Send play sound effect command to {All}, number: {number}");
            GetPlayers().ForEach(player => player.SendPlaySoundEffectCommand(number));
        }

        public void SendAnsweringTimerData(AnsweringTimerData data)
        {
            //Debug.Log($"Master: Send answering timer data to {All}, {data}");
            GetPlayers().ForEach(player => player.SendAnsweringTimerData(data));
        }

        public void SendAuctionData(AuctionData auctionData)
        {
            Debug.Log($"Master: Send AuctionData to {All}: {auctionData}");
            GetPlayers().ForEach(player => player.SendAuctionData(auctionData));
        }

        public void SendFinalRoundData(FinalRoundData finalRoundData)
        {
            Debug.Log($"Master: Send FinalRoundData to {All}: {finalRoundData}");
            GetPlayers().ForEach(player => player.SendFinalRoundData(finalRoundData));
        }

        public void SendCommand(IndividualPlayerCommand command)
        {
            Debug.Log($"Master: Send individual player command to {command.Receiver}");
            NetworkPlayer networkPlayer = GetPlayer(command.Receiver);
            networkPlayer.SendCommandToPlayer(command);
        }
    }
}