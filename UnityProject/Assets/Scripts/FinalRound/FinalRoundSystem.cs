using System;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class FinalRoundSystem
    {
        [Inject] private FinalRoundData Data { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MessageDialogueView MessageDialogueView { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private SendToMasterService SendToMasterService { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        
        public void Select(Round round)
        {
            Data.Round = round;
            Reset();
        }

        public void Reset()
        {
            Data.Reset(Data.Round.Themes.Select(_ => _.Name).ToArray());
            Data.SetPhase(FinalRoundPhase.ThemesRemoving);
            if (CanAnyParticipate())
            {
                SelectFirstPlayerForCrossOut();
            }
            else
            {
                PlayersBoard.SetCurrent(null);
                MessageDialogueView.Show("Некому учавствовать!", "Ни один из игроков не может принять участие в финальном раунде!");
            }
        }
        
        public bool CanAnyParticipate()
        {
            return PlayersBoard.Players.Any(CanParticipate);
        }
        
        private void SelectFirstPlayerForCrossOut()
        {
            if (PlayersBoard.Current == null || !CanParticipate(PlayersBoard.Current))
                PlayersBoard.SetCurrent(PlayersBoard.Players.First(CanParticipate));
        }

        private void SelectNextPlayerForCrossOut()
        {
            int index = PlayersBoard.Players.IndexOf(PlayersBoard.Current);
            int i = index;
            for (;;)
            {
                i = (i + 1) % PlayersBoard.Players.Count;
                
                if (i == index)
                    break;

                if (CanParticipate(PlayersBoard.Players[i]))
                {
                    PlayersBoard.SetCurrent(PlayersBoard.Players[i]);
                    break;
                }
            }
        }

        public bool CanParticipate(PlayerData player)
        {
            return player.Score > 0;
        }

        public void TryRemoveTheme(int index)
        {
            if (NetworkData.IsMaster)
            {
                Debug.Log($"Master. Remove theme '{index}' for player '{PlayersBoard.Current}'");
                RemoveTheme(index);
            }

            if (NetworkData.IsClient)
            {
                if (MatchData.IsMeCurrentPlayer)
                    SendToMasterService.SendRemoveTheme(index);
                else
                    Debug.Log($"Can't cross out theme as this player '{MatchData.ThisPlayer}' is not current '{PlayersBoard.Current}'");
            }
        }

        public void MasterOnReceiveRemoveTheme(PlayerData player, int index)
        {
            if (PlayersBoard.Current == player)
                RemoveTheme(index);
            else
                Debug.Log($"Player '{player}' can't remove theme '{index}' as they is not current");
        }

        private void RemoveTheme(int index)
        {
            int remainedThemesAmount = GetRemainedThemesAmount();
            if (remainedThemesAmount <= 1)
            {
                Debug.Log($"Can't remove any theme anymore, remained themes amount: {remainedThemesAmount}");
                return;
            }

            if (index < 0 || index >= Data.RemovedThemes.Length)
            {
                Debug.Log($"Not correct remove theme index '{index}'. Length is '{Data.RemovedThemes.Length}'");
                return;
            }

            if (Data.RemovedThemes[index])
            {
                Debug.Log($"Theme by index '{index}' was removed before");
                return;
            }

            Debug.Log($"Remove theme by index: {index}");
            Data.RemoveTheme(index);

            if (IsRemovingFinished())
                PlayersBoard.SetCurrent(null);

            if (PlayersBoard.Current != null)
                SelectNextPlayerForCrossOut();
        }

        private int GetRemainedThemesAmount()
        {
            return Data.RemovedThemes.Count(isRemoved => !isRemoved);
        }

        public bool IsRemovingFinished()
        {
            return GetRemainedThemesAmount() == 1;
        }

        public string GetSelectedTheme()
        {
            for (int i = 0; i < Data.RemovedThemes.Length; i++)
                if (!Data.RemovedThemes[i])
                    return Data.Themes[i];

            throw new Exception($"All themes are removed: {Data}");
        }

        public void ChangePhase(FinalRoundPhase phase)
        {
            Debug.Log($"FinalRound change phase: {phase}");
            Data.SetPhase(phase);

            if (phase == FinalRoundPhase.Betting)
            {
                Data.ClearBets(PlayersBoard.Players.Count);
                Data.SetDoneBets(PlayersBoard.Players.Select(player => !CanParticipate(player)).ToArray());
            }
        }
        
        #region Phase 2: Betting

        public void TryMakeBet(int bet)
        {
            if (NetworkData.IsClient)
                SendToMasterService.SendFinalRoundBet(bet);

            if (NetworkData.IsMaster && Data.SelectedPlayerByMaster != null)
                MakeBet(Data.SelectedPlayerByMaster, bet);
        }

        public void TryMakeAllInBet()
        {
            if (NetworkData.IsClient)
                SendToMasterService.SendFinalRoundBet(MatchData.ThisPlayer.Score);

            if (NetworkData.IsMaster && Data.SelectedPlayerByMaster != null)
                MakeBet(Data.SelectedPlayerByMaster, Data.SelectedPlayerByMaster.Score);
        }

        public void MasterOnReceiveBet(PlayerData player, int bet)
        {
            if (bet <= 0 || bet > player.Score)
            {
                Debug.Log($"Can't accept bet '{bet}' from player '{player}'.");
                return;
            }

            MakeBet(player, bet);
        }

        private void MakeBet(PlayerData player, int bet)
        {
            Debug.Log($"Make bet '{bet}' for player '{player}'");
            int index = PlayersBoard.Players.IndexOf(player);
            Data.SetBet(index, bet);
        }
        
        #endregion
    }
}