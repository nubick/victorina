using Injection;
using UnityEngine;

namespace Victorina
{
    public class AuctionSystem
    {
        [Inject] private SendToMasterService SendToMasterService { get; set; }
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
        private AuctionData Data => QuestionAnswerData.AuctionData.Value;
        private PlayersBoard PlayersBoard => MatchData.PlayersBoard.Value;

        public void StartNew(PlayerData currentPlayer, int questionPrice)
        {
            Data.Bet = questionPrice;
            Data.Player = currentPlayer;
            Data.BettingPlayer = currentPlayer;
            Data.IsAllIn = false;
            Data.PassedPlayers.Clear();
            Data.Player = null;
            SendToPlayersService.SendAuctionData(Data);
            QuestionAnswerData.AuctionData.NotifyChanged();
        }
        
        public void SendPlayerPass()
        {
            SendToMasterService.SendPassAuction();
        }
        
        public void SendPlayerAllIn()
        {
            SendToMasterService.SendAllInAuction();
        }

        public void SendPlayerBet(int bet)
        {
            SendToMasterService.SendBetAuction(bet);
        }

        private PlayerData SelectNextBettingPlayer()
        {
            int lastIndex = PlayersBoard.Players.IndexOf(Data.BettingPlayer);
            for (int index = lastIndex + 1; index % PlayersBoard.Players.Count != lastIndex; index++)
            {
                PlayerData player = PlayersBoard.Players[index];
                if (!Data.PassedPlayers.Contains(player))
                    return player;
            }
            return null;
        }
        
        public void MasterOnReceivePlayerPass(PlayerData player)
        {
            bool cantPass = Data.Player == null && Data.BettingPlayer == player;
            if (cantPass)
            {
                Debug.Log($"Player '{player}' can't pass as it is betting player and player was not selected yet.");
                return;
            }
            
            if (Data.PassedPlayers.Contains(player))
            {
                Debug.Log($"Receive pass request from player '{player}' who passed before");
            }
            else
            {
                Data.PassedPlayers.Add(player);
                if (Data.BettingPlayer == player)
                    Data.BettingPlayer = SelectNextBettingPlayer();
                SendToPlayersService.SendAuctionData(Data);
                QuestionAnswerData.AuctionData.NotifyChanged();
            }
        }

        public void MasterOnReceivePlayerAllIn(PlayerData player)
        {
            bool canAllIn = Data.BettingPlayer == player && player.Score > Data.Bet;
            if (canAllIn)
            {
                Data.IsAllIn = true;
                Data.Bet = player.Score;
                Data.BettingPlayer = SelectNextBettingPlayer();
                SendToPlayersService.SendAuctionData(Data);
                QuestionAnswerData.AuctionData.NotifyChanged();
            }
            else
            {
                Debug.Log($"Player '{player}' can't all in {Data}");
            }
        }

        public void MasterOnReceivePlayerBet(PlayerData player, int bet)
        {
            bool canBet = Data.BettingPlayer == player && !Data.IsAllIn && bet <= player.Score && bet > Data.Bet;
            if (canBet)
            {
                Data.Bet = bet;
                Data.Player = player;
                Data.BettingPlayer = SelectNextBettingPlayer();
                SendToPlayersService.SendAuctionData(Data);
                QuestionAnswerData.AuctionData.NotifyChanged();
            }
            else
            {
                Debug.Log($"Player '{player}' can't do bet '{bet}', {Data}");
            }
        }

        public void MasterFinishAuction()
        {
            QuestionAnswerSystem.ShowNext();
        }
    }
}