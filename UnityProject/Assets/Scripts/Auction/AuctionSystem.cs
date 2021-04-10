using System;
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
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        
        private AuctionData Data => QuestionAnswerData.AuctionData.Value;
        private PlayersBoard PlayersBoard => MatchData.PlayersBoard.Value;

        public void StartNew(PlayerData currentPlayer, int questionPrice)
        {
            Data.Bet = questionPrice;
            Data.Player = null;
            Data.BettingPlayer = currentPlayer;
            Data.IsAllIn = false;
            Data.PassedPlayers.Clear();
            AddAutomaticPasses();
            
            SendToPlayersService.SendAuctionData(Data);
            QuestionAnswerData.AuctionData.NotifyChanged();
        }
        
        #region Client
        
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

        #endregion

        private PlayerData SelectNextBettingPlayer()
        {
            int lastIndex = PlayersBoard.Players.IndexOf(Data.BettingPlayer);
            for (int dIndex = 1; dIndex <= PlayersBoard.Players.Count; dIndex++)
            {
                PlayerData player = PlayersBoard.Players[(lastIndex + dIndex) % PlayersBoard.Players.Count];
                if (!Data.PassedPlayers.Contains(player))
                    return player;
            }
            throw new Exception("Logic error! It looks like all players are passed.");
        }

        public void MasterOnReceivePlayerPass(PlayerData player)
        {
            if (!CanPass(player))
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
            bool canAllIn = Data.BettingPlayer == player && player.Score >= Data.NextMinBet;
            if (canAllIn)
            {
                Data.IsAllIn = true;
                Data.Bet = player.Score;
                Data.Player = player;
                AddAutomaticPasses();
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
            bool isForceBet = Data.Player == null && player.Score < bet;
            bool canBet = Data.BettingPlayer == player && !Data.IsAllIn && bet >= Data.NextMinBet &&
                          (player.Score >= bet || isForceBet);
            if (canBet)
            {
                Data.Bet = bet;
                Data.Player = player;
                AddAutomaticPasses();
                Data.BettingPlayer = SelectNextBettingPlayer();
                
                SendToPlayersService.SendAuctionData(Data);
                QuestionAnswerData.AuctionData.NotifyChanged();
            }
            else
            {
                Debug.Log($"Player '{player}' can't do bet '{bet}', {Data}");
            }
        }

        private void AddAutomaticPasses()
        {
            foreach (PlayerData player in PlayersBoard.Players)
            {
                if(Data.PassedPlayers.Contains(player))
                    continue;

                if(!CanPass(player))
                    continue;
                
                if (player.Score < Data.NextMinBet)
                    Data.PassedPlayers.Add(player);
            }
        }

        public bool CanPass(PlayerData player)
        {
            bool canPass1 = Data.Player != player;//Player is not who hold bet
            bool canPass2 = !(Data.Player == null && Data.BettingPlayer == player);
            return canPass1 && canPass2;
        }
        
        public void MasterFinishAuction()
        {
            if (NetworkData.IsMaster)
            {
                PlayersBoardSystem.MakePlayerCurrent(Data.BettingPlayer);
                QuestionAnswerSystem.ShowNext();
            }
            else
            {
                Debug.LogWarning("Player clicked finish auction button");
            }
        }
    }
}