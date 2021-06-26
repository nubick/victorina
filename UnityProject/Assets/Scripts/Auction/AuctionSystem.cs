using System;
using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class AuctionSystem
    {
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        private AuctionPlayState AuctionPlayState => PlayStateData.As<AuctionPlayState>();
        
        public void StartNew(int questionPrice, string theme)
        {
            AuctionPlayState.Bet = questionPrice;
            AuctionPlayState.Theme = theme;
            AuctionPlayState.Player = null;
            AuctionPlayState.BettingPlayer = PlayersBoard.Current;
            AuctionPlayState.IsAllIn = false;
            AuctionPlayState.PassedPlayers.Clear();
            AuctionPlayState.SelectedPlayerByMaster = null;
            AddAutomaticPasses();
        }
        
        public void SendPlayerPass()
        {
            CommandsSystem.AddNewCommand(new PassAuctionCommand());
        }
        
        public void SendPlayerAllIn()
        {
            CommandsSystem.AddNewCommand(new MakeAllInAuctionCommand());
        }

        public void SendPlayerBet(int bet)
        {
            if (NetworkData.IsClient)
                CommandsSystem.AddNewCommand(new MakeBetAuctionCommand {Bet = bet});

            if (NetworkData.IsMaster)
                CommandsSystem.AddNewCommand(new MakeBetForPlayerAuctionCommand {Bet = bet, Player = AuctionPlayState.SelectedPlayerByMaster});
        }

        public PlayerData SelectNextBettingPlayer()
        {
            int lastIndex = PlayersBoard.Players.IndexOf(AuctionPlayState.BettingPlayer);
            for (int dIndex = 1; dIndex <= PlayersBoard.Players.Count; dIndex++)
            {
                PlayerData player = PlayersBoard.Players[(lastIndex + dIndex) % PlayersBoard.Players.Count];
                if (!AuctionPlayState.PassedPlayers.Contains(player))
                    return player;
            }
            throw new Exception("Logic error! It looks like all players are passed.");
        }
        
        public void AddAutomaticPasses()
        {
            foreach (PlayerData player in PlayersBoard.Players)
            {
                if(AuctionPlayState.PassedPlayers.Contains(player))
                    continue;

                if(!CanPass(player))
                    continue;
                
                if (player.Score < AuctionPlayState.NextMinBet)
                    AuctionPlayState.PassedPlayers.Add(player);
            }
        }

        public bool CanPass(PlayerData player)
        {
            bool canPass1 = AuctionPlayState.Player != player;//Player is not who hold bet
            bool canPass2 = !(AuctionPlayState.Player == null && AuctionPlayState.BettingPlayer == player);
            return canPass1 && canPass2;
        }
        
        public void MasterFinishAuction()
        {
            CommandsSystem.AddNewCommand(new FinishAuctionCommand());
        }
    }
}