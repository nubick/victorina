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
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MessageDialogueView MessageDialogueView { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private PlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }

        private FinalRoundPlayState PlayState => PlayStateData.As<FinalRoundPlayState>();
        
        public void Reset()
        {
            PlayState.Reset(PlayState.Round.Themes.Select(_ => _.Name).ToArray());
            PlayState.SetPhase(FinalRoundPhase.ThemesRemoving);
            if (CanAnyParticipate())
            {
                SelectFirstPlayerForRemoving();
            }
            else
            {
                PlayersBoard.SetCurrent(null);
                MessageDialogueView.Show("Некому учавствовать!", "Ни один из игроков не может принять участие в финальном раунде!");
            }
            
            ServerEvents.FinalRoundStarted.Publish();
        }
        
        public bool CanAnyParticipate()
        {
            return PlayersBoard.Players.Any(CanParticipate);
        }
        
        public bool CanParticipate(PlayerData player)
        {
            return player.Score > 0;
        }
        
        public string GetSelectedTheme()
        {
            return PlayState.Themes[GetSelectedThemeIndex()];
        }

        private int GetSelectedThemeIndex()
        {
            for (int i = 0; i < PlayState.RemovedThemes.Length; i++)
                if (!PlayState.RemovedThemes[i])
                    return i;

            throw new Exception($"All themes are removed: {PlayState}");
        }
        
        public void ChangePhase(FinalRoundPhase phase)
        {
            Debug.Log($"FinalRound change phase: {phase}");
            PlayState.SetPhase(phase);

            if (phase == FinalRoundPhase.Betting)
            {
                PlayState.ClearBets(PlayersBoard.Players.Count);
                PlayState.SetDoneBets(PlayersBoard.Players.Select(player => !CanParticipate(player)).ToArray());
            }
            else if (phase == FinalRoundPhase.QuestionShowing)
            {
                ShowFinalRoundQuestion();
            }
            else if (phase == FinalRoundPhase.Answering)
            {
                PlayState.ClearAnswers(PlayersBoard.Players.Count);
                PlayState.SetDoneAnswers(PlayersBoard.Players.Select(player => !CanParticipate(player)).ToArray());
            }
            else if (phase == FinalRoundPhase.AnswersAccepting)
            {
                StartAnswersAcceptingPhase();
            }
        }
        
        #region Phase 1: Themes Removing
        
        public void TryRemoveTheme(int index)
        {
            CommandsSystem.AddNewCommand(new RemoveFinalRoundThemeCommand {ThemeIndex = index});
        }
        
        private void SelectFirstPlayerForRemoving()
        {
            if (PlayersBoard.Current == null || !CanParticipate(PlayersBoard.Current))
                PlayersBoard.SetCurrent(PlayersBoard.Players.First(CanParticipate));
        }

        #endregion
        
        #region Phase 2: Betting

        public void TryMakeBet(int bet)
        {
            CommandsSystem.AddNewCommand(new MakeFinalRoundBetCommand {Bet = bet});
        }

        public void TryMakeAllInBet()
        {
            if (NetworkData.IsClient)
                TryMakeBet(MatchData.ThisPlayer.Score);

            if (NetworkData.IsMaster && PlayState.SelectedPlayerByMaster != null)
                TryMakeBet(PlayState.SelectedPlayerByMaster.Score);
        }
        
        #endregion

        #region Phase 3: Show Question

        private void ShowFinalRoundQuestion()
        {
            ShowFinalRoundQuestionPlayState showQuestionPlayState = new ShowFinalRoundQuestionPlayState();
            showQuestionPlayState.FinalRoundPlayState = PlayState;
            showQuestionPlayState.NetQuestion = GetSelectedNetQuestion();
            PlayStateSystem.ChangePlayState(showQuestionPlayState);
        }

        private NetQuestion GetSelectedNetQuestion()
        {
            Theme theme = PlayState.Round.Themes[GetSelectedThemeIndex()];
            return PackageSystem.BuildNetQuestion(theme.Questions.First().Id);
        }
        
        public void SwitchToAnsweringPhase()
        {
            FinalRoundPlayState finalRoundPlayState = PlayStateData.As<ShowFinalRoundQuestionPlayState>().FinalRoundPlayState;
            PlayStateSystem.ChangePlayState(finalRoundPlayState);
            ChangePhase(FinalRoundPhase.Answering);
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
            if (PlayState.SelectedPlayerByMaster == null)
                throw new Exception("Should not be possible to click Clear Answer button when player is not selected.");
            
            CommandsSystem.AddNewCommand(new ClearFinalRoundAnswerCommand {Player = PlayState.SelectedPlayerByMaster});
        }
        
        #endregion
        
        #region Phase 5: Answers Accepting

        private PlayerData AcceptingPlayer => PlayersBoard.Players[PlayState.AcceptingPlayerIndex];
        
        private void StartAnswersAcceptingPhase()
        {
            PlayState.AcceptingPlayerIndex = -1;
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
            int? nextIndex = GetNextAcceptingPlayerIndex(PlayState.AcceptingPlayerIndex);
            if (nextIndex.HasValue)
            {
                PlayState.AcceptingPlayerIndex = nextIndex.Value;
                PlayState.AcceptingPhase = FinalRoundAcceptingPhase.Name;
            }
            else
            {
                PlayState.AcceptingPhase = FinalRoundAcceptingPhase.Finish;
            }
            RefreshAcceptingInfo();
        }
        
        public void ShowAcceptingAnswer()
        {
            PlayState.AcceptingPhase = FinalRoundAcceptingPhase.Answer;
            RefreshAcceptingInfo();
        }
        
        public void AcceptAnswerAsCorrect()
        {
            PlayState.IsAcceptedAsCorrect = true;
            PlayState.AcceptingPhase = FinalRoundAcceptingPhase.Result;
            RefreshAcceptingInfo();
        }

        public void AcceptAnswerAsWrong()
        {
            PlayState.IsAcceptedAsCorrect = false;
            PlayState.AcceptingPhase = FinalRoundAcceptingPhase.Result;
            RefreshAcceptingInfo();
        }

        public void ApplyPlayerBet()
        {
            PlayState.AcceptingPhase = FinalRoundAcceptingPhase.Bet;
            
            int bet = PlayState.Bets[PlayState.AcceptingPlayerIndex];
            if (PlayState.IsAcceptedAsCorrect)
                PlayersBoardSystem.RewardPlayer(AcceptingPlayer, bet);
            else
                PlayersBoardSystem.FinePlayer(AcceptingPlayer, bet);

            int? nextIndex = GetNextAcceptingPlayerIndex(PlayState.AcceptingPlayerIndex);
            if (nextIndex == null)
                PlayState.AcceptingPhase = FinalRoundAcceptingPhase.Finish;
            
            RefreshAcceptingInfo();
        }
        
        private void RefreshAcceptingInfo()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine(PlayersBoard.Players[PlayState.AcceptingPlayerIndex].Name);

            if (PlayState.AcceptingPhase >= FinalRoundAcceptingPhase.Answer)
                sb.AppendLine(PlayState.Answers[PlayState.AcceptingPlayerIndex]);

            if (PlayState.AcceptingPhase >= FinalRoundAcceptingPhase.Result)
            {
                string result = PlayState.IsAcceptedAsCorrect ? "Верно" : "Неверно";
                sb.AppendLine(result);
            }

            if (PlayState.AcceptingPhase >= FinalRoundAcceptingPhase.Bet)
                sb.AppendLine(PlayState.Bets[PlayState.AcceptingPlayerIndex].ToString());
            
            PlayState.SetAcceptingInfo(sb.ToString());
        }

        public void FinishRound()
        {
            ResultPlayState resultPlayState = new ResultPlayState();
            PlayStateSystem.ChangePlayState(resultPlayState);
        }

        #endregion
    }
}