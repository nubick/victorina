using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class MatchSystem
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        
        public void Start()
        {
            SelectRound(1);
        }
        
        public void TrySelectQuestion(NetRoundQuestion netRoundQuestion)
        {
            if (netRoundQuestion.IsAnswered)
            {
                Debug.Log($"Selected question is answered: {netRoundQuestion}");
            }
            else if (NetworkData.IsMaster)
            {
                SelectQuestion(netRoundQuestion);
            }
            else
            {
                Debug.Log($"Not admin can't select question: {netRoundQuestion}");
            }
        }

        private void SelectQuestion(NetRoundQuestion netRoundQuestion)
        {
            QuestionTimer.Reset(Static.TimeForAnswer);
            MatchData.WasTimerStarted = false;
            MatchData.IsTimerOn = false;
            
            MatchData.SelectedQuestion.Value = BuildNetQuestion(netRoundQuestion);
            SendToPlayersService.SendSelectedQuestion(MatchData.SelectedQuestion.Value);

            MatchData.SelectedRoundQuestion = netRoundQuestion;
            SendToPlayersService.SendSelectedRoundQuestion(MatchData.SelectedRoundQuestion);

            MatchData.Phase.Value = MatchPhase.ShowQuestion;
            SendToPlayersService.Send(MatchData.Phase.Value);

            MatchData.CurrentStoryDotIndex.Value = 0;
            SendToPlayersService.SendCurrentStoryDotIndex(MatchData.CurrentStoryDotIndex.Value);
            
            StartTimerIfTime();
        }

        private NetQuestion BuildNetQuestion(NetRoundQuestion netRoundQuestion)
        {
            Question question = PackageSystem.GetQuestion(netRoundQuestion.QuestionId);
            NetQuestion netQuestion = new NetQuestion();
            netQuestion.QuestionStory = question.QuestionStory.ToArray();
            netQuestion.QuestionStoryDotsAmount = netQuestion.QuestionStory.Length;
            netQuestion.AnswerStory = question.AnswerStory.ToArray();
            netQuestion.AnswerStoryDotsAmount = netQuestion.AnswerStory.Length;
            return netQuestion;
        }
        
        public void BackToRound()
        {
            PackageData.PackageProgress.SetQuestionAsAnswered(MatchData.SelectedRoundQuestion.QuestionId);
            SelectRound(MatchData.RoundsInfo.Value.CurrentRoundNumber );
        }

        public void SelectRound(int number)
        {
            if (NetworkData.IsClient)
                return;

            MatchData.RoundsInfo.Value.RoundsAmount = PackageData.Package.Rounds.Count;
            MatchData.RoundsInfo.Value.CurrentRoundNumber = number;
            SendToPlayersService.Send(MatchData.RoundsInfo.Value);

            Round round = PackageData.Package.Rounds[number - 1];
            MatchData.RoundData.Value = BuildNetRound(round, PackageData.PackageProgress);
            SendToPlayersService.Send(MatchData.RoundData.Value);

            MatchData.Phase.Value = MatchPhase.Round;
            SendToPlayersService.Send(MatchData.Phase.Value);

            (int[] fileIds, int[] chunksAmounts) info = PackageSystem.GetRoundFileIds(round);
            SendToPlayersService.SendRoundFileIds(info.fileIds, info.chunksAmounts);
        }

        private NetRound BuildNetRound(Round round, PackageProgress packageProgress)
        {
            NetRound netRound = new NetRound();
            foreach (Theme theme in round.Themes)
            {
                NetRoundTheme netRoundTheme = new NetRoundTheme();
                netRoundTheme.Name = theme.Name;
                foreach (Question question in theme.Questions)
                {
                    NetRoundQuestion netRoundQuestion = new NetRoundQuestion(question.Id);
                    netRoundQuestion.Price = question.Price;
                    netRoundQuestion.IsAnswered = packageProgress.IsAnswered(question.Id);
                    netRoundTheme.Questions.Add(netRoundQuestion);
                }
                netRound.Themes.Add(netRoundTheme);
            }
            return netRound;
        }

        public void ShowNext()
        {
            MatchData.CurrentStoryDotIndex.Value++;
            SendToPlayersService.SendCurrentStoryDotIndex(MatchData.CurrentStoryDotIndex.Value);
            StartTimerIfTime();
        }

        private void StartTimerIfTime()
        {
            if (MatchData.WasTimerStarted)
                return;
            
            if (MatchData.Phase.Value == MatchPhase.ShowQuestion &&
                MatchData.CurrentStoryDot == MatchData.SelectedQuestion.Value.QuestionStory.Last())
                StartTimer();
        }

        public void ShowPrevious()
        {
            MatchData.CurrentStoryDotIndex.Value--;
            SendToPlayersService.SendCurrentStoryDotIndex(MatchData.CurrentStoryDotIndex.Value);
        }

        public void StartTimer()
        {
            QuestionTimer.Start();
            
            MatchData.WasTimerStarted = true;
            MatchData.IsTimerOn = true;
            SendToPlayersService.SendStartTimer();
        }

        public void StopTimer()
        {
            QuestionTimer.Stop();
            
            MatchData.IsTimerOn = false;
            SendToPlayersService.SendStopTimer();
        }
        
        public void ShowAnswer()
        {
            StopTimer();
            MatchData.Phase.Value = MatchPhase.ShowAnswer;
            SendToPlayersService.Send(MatchData.Phase.Value);

            MatchData.CurrentStoryDotIndex.Value = 0;
            SendToPlayersService.SendCurrentStoryDotIndex(MatchData.CurrentStoryDotIndex.Value);
        }
    }
}