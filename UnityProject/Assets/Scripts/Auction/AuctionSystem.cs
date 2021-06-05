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
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        
        private AuctionData AuctionData => QuestionAnswerData.AuctionData.Value;

        public void StartNew(PlayerData currentPlayer, int questionPrice)
        {
            AuctionData.Bet = questionPrice;
            AuctionData.Player = null;
            AuctionData.BettingPlayer = currentPlayer;
            AuctionData.IsAllIn = false;
            AuctionData.PassedPlayers.Clear();
            AuctionData.SelectedPlayerByMaster = null;
            AddAutomaticPasses();
            
            SendToPlayersService.SendAuctionData(AuctionData);
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
            if (NetworkData.IsClient)
                SendToMasterService.SendBetAuction(bet);

            if (NetworkData.IsMaster)
                MasterMakeBetForPlayer(bet, AuctionData.SelectedPlayerByMaster);
        }

        #endregion

        private PlayerData SelectNextBettingPlayer()
        {
            int lastIndex = PlayersBoard.Players.IndexOf(AuctionData.BettingPlayer);
            for (int dIndex = 1; dIndex <= PlayersBoard.Players.Count; dIndex++)
            {
                PlayerData player = PlayersBoard.Players[(lastIndex + dIndex) % PlayersBoard.Players.Count];
                if (!AuctionData.PassedPlayers.Contains(player))
                    return player;
            }
            throw new Exception("Logic error! It looks like all players are passed.");
        }

        private void MasterMakeBetForPlayer(int bet, PlayerData player)
        {
            AuctionData.Bet = bet;
            AuctionData.IsAllIn = false;
            AuctionData.Player = player;
            AuctionData.PassedPlayers.Remove(player);
            AuctionData.BettingPlayer = player;
            AddAutomaticPasses();
            AuctionData.BettingPlayer = SelectNextBettingPlayer();

            SendToPlayersService.SendAuctionData(AuctionData);
            QuestionAnswerData.AuctionData.NotifyChanged();
        }

        public void MasterOnReceivePlayerPass(PlayerData player)
        {
            if (!CanPass(player))
            {
                Debug.Log($"Player '{player}' can't pass as it is betting player and player was not selected yet.");
                return;
            }
            
            if (AuctionData.PassedPlayers.Contains(player))
            {
                Debug.Log($"Receive pass request from player '{player}' who passed before");
            }
            else
            {
                AuctionData.PassedPlayers.Add(player);
                if (AuctionData.BettingPlayer == player)
                    AuctionData.BettingPlayer = SelectNextBettingPlayer();

                SendToPlayersService.SendAuctionData(AuctionData);
                QuestionAnswerData.AuctionData.NotifyChanged();
            }
        }

        public void MasterOnReceivePlayerAllIn(PlayerData player)
        {
            bool canAllIn = AuctionData.BettingPlayer == player && player.Score >= AuctionData.NextMinBet;
            if (canAllIn)
            {
                AuctionData.IsAllIn = true;
                AuctionData.Bet = player.Score;
                AuctionData.Player = player;
                AddAutomaticPasses();
                AuctionData.BettingPlayer = SelectNextBettingPlayer();
                
                SendToPlayersService.SendAuctionData(AuctionData);
                QuestionAnswerData.AuctionData.NotifyChanged();
            }
            else
            {
                Debug.Log($"Player '{player}' can't all in {AuctionData}");
            }
        }

        public void MasterOnReceivePlayerBet(PlayerData player, int bet)
        {
            bool isForceBet = AuctionData.Player == null && player.Score < bet;
            bool canBet = AuctionData.BettingPlayer == player && !AuctionData.IsAllIn && bet >= AuctionData.NextMinBet &&
                          (player.Score >= bet || isForceBet);
            if (canBet)
            {
                AuctionData.Bet = bet;
                AuctionData.Player = player;
                AddAutomaticPasses();
                AuctionData.BettingPlayer = SelectNextBettingPlayer();
                
                SendToPlayersService.SendAuctionData(AuctionData);
                QuestionAnswerData.AuctionData.NotifyChanged();
            }
            else
            {
                Debug.Log($"Player '{player}' can't do bet '{bet}', {AuctionData}");
            }
        }

        private void AddAutomaticPasses()
        {
            foreach (PlayerData player in PlayersBoard.Players)
            {
                if(AuctionData.PassedPlayers.Contains(player))
                    continue;

                if(!CanPass(player))
                    continue;
                
                if (player.Score < AuctionData.NextMinBet)
                    AuctionData.PassedPlayers.Add(player);
            }
        }

        public bool CanPass(PlayerData player)
        {
            bool canPass1 = AuctionData.Player != player;//Player is not who hold bet
            bool canPass2 = !(AuctionData.Player == null && AuctionData.BettingPlayer == player);
            return canPass1 && canPass2;
        }
        
        public void MasterFinishAuction()
        {
            if (NetworkData.IsMaster)
            {
                if (AuctionData.Player == null)
                {
                    Debug.LogWarning("Auction player is null. Can't finish auction.");
                }
                else
                {
                    QuestionAnswerData.AdmittedPlayersIds.Add(AuctionData.Player.PlayerId);
                    PlayersBoardSystem.MakePlayerCurrent(AuctionData.Player);
                    QuestionAnswerSystem.StartQuestionStory();
                }
            }
            else
            {
                Debug.LogWarning("Player clicked finish auction button");
            }
        }
    }
}