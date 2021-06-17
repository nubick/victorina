using System;
using System.Linq;
using System.Text;
using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class FinalRoundSystem
    {
        [Inject] private FinalRoundData Data { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MessageDialogueView MessageDialogueView { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
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
        
        public bool CanParticipate(PlayerData player)
        {
            return player.Score > 0;
        }

        public void TryRemoveTheme(int index)
        {
            CommandsSystem.AddNewCommand(new RemoveFinalRoundThemeCommand {ThemeIndex = index});
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
            else if (phase == FinalRoundPhase.Answering)
            {
                Data.ClearAnswers(PlayersBoard.Players.Count);
                Data.SetDoneAnswers(PlayersBoard.Players.Select(player => !CanParticipate(player)).ToArray());
            }
            else if (phase == FinalRoundPhase.AnswersAccepting)
            {
                StartAnswersAcceptingPhase();
            }
        }
        
        #region Phase 2: Betting

        public void TryMakeBet(int bet)
        {
            CommandsSystem.AddNewCommand(new MakeFinalRoundBetCommand {Bet = bet});
        }

        public void TryMakeAllInBet()
        {
            if (NetworkData.IsClient)
                TryMakeBet(MatchData.ThisPlayer.Score);

            if (NetworkData.IsMaster && Data.SelectedPlayerByMaster != null)
                TryMakeBet(Data.SelectedPlayerByMaster.Score);
        }
        
        #endregion

        #region Phase 4: Answering
        
        public void SendAnswer(string answerText)
        {
            if (string.IsNullOrWhiteSpace(answerText))
                MessageDialogueView.Show("Нет ответа?", "Ну отправьте хоть что-нибудь!");
            else
                CommandsSystem.AddNewCommand(new SendFinalRoundAnswerCommand {AnswerText = answerText});
        }

        public void ClearAnswer()
        {
            if (Data.SelectedPlayerByMaster == null)
                throw new Exception("Should not be possible to click Clear Answer button when player is not selected.");
            
            CommandsSystem.AddNewCommand(new ClearFinalRoundAnswerCommand {Player = Data.SelectedPlayerByMaster});
        }
        
        #endregion
        
        #region Phase 5: Answers Accepting

        private PlayerData AcceptingPlayer => PlayersBoard.Players[Data.AcceptingPlayerIndex];
        
        private void StartAnswersAcceptingPhase()
        {
            Data.AcceptingPlayerIndex = -1;
            SwitchToNextAcceptingPlayer();
            RefreshAcceptingInfo();
        }

        private int? GetNextAcceptingPlayerIndex(int currentIndex)
        {
            for (;;)
            {
                currentIndex++;

                if (currentIndex == PlayersBoard.Players.Count)
                    return null;

                if (CanParticipate(PlayersBoard.Players[currentIndex]))
                    return currentIndex;
            }
        }

        public void SwitchToNextAcceptingPlayer()
        {
            int? nextIndex = GetNextAcceptingPlayerIndex(Data.AcceptingPlayerIndex);
            if (nextIndex.HasValue)
            {
                Data.AcceptingPlayerIndex = nextIndex.Value;
                Data.AcceptingPhase = FinalRoundAcceptingPhase.Name;
            }
            else
            {
                Data.AcceptingPhase = FinalRoundAcceptingPhase.Finish;
            }
            RefreshAcceptingInfo();
        }
        
        public void ShowAcceptingAnswer()
        {
            Data.AcceptingPhase = FinalRoundAcceptingPhase.Answer;
            RefreshAcceptingInfo();
        }
        
        public void AcceptAnswerAsCorrect()
        {
            Data.IsAcceptedAsCorrect = true;
            Data.AcceptingPhase = FinalRoundAcceptingPhase.Result;
            RefreshAcceptingInfo();
        }

        public void AcceptAnswerAsWrong()
        {
            Data.IsAcceptedAsCorrect = false;
            Data.AcceptingPhase = FinalRoundAcceptingPhase.Result;
            RefreshAcceptingInfo();
        }

        public void ApplyPlayerBet()
        {
            Data.AcceptingPhase = FinalRoundAcceptingPhase.Bet;
            
            int bet = Data.Bets[Data.AcceptingPlayerIndex];
            if (Data.IsAcceptedAsCorrect)
                MatchSystem.RewardPlayer(AcceptingPlayer, bet);
            else
                MatchSystem.FinePlayer(AcceptingPlayer, bet);

            int? nextIndex = GetNextAcceptingPlayerIndex(Data.AcceptingPlayerIndex);
            if (nextIndex == null)
                Data.AcceptingPhase = FinalRoundAcceptingPhase.Finish;
            
            RefreshAcceptingInfo();
        }
        
        private void RefreshAcceptingInfo()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine(PlayersBoard.Players[Data.AcceptingPlayerIndex].Name);

            if (Data.AcceptingPhase >= FinalRoundAcceptingPhase.Answer)
                sb.AppendLine(Data.Answers[Data.AcceptingPlayerIndex]);

            if (Data.AcceptingPhase >= FinalRoundAcceptingPhase.Result)
            {
                string result = Data.IsAcceptedAsCorrect ? "Верно" : "Неверно";
                sb.AppendLine(result);
            }

            if (Data.AcceptingPhase >= FinalRoundAcceptingPhase.Bet)
                sb.AppendLine(Data.Bets[Data.AcceptingPlayerIndex].ToString());
            
            Data.SetAcceptingInfo(sb.ToString());
        }
        
        public void FinishRound()
        {
            
        }
        
        #endregion
    }
}