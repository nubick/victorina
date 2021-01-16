using System.Linq;
using Injection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Victorina
{
    public class MatchSystem
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        
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
            else if (NetworkData.IsAdmin)
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
            Question question = PackageSystem.GetQuestion(netRoundQuestion.QuestionId);

            NetQuestion netQuestion = new NetQuestion();
            netQuestion.QuestionStory = question.QuestionStory.ToArray();
            netQuestion.StoryDotsAmount = netQuestion.QuestionStory.Length;
            netQuestion.Answer = question.Answer;
            
            MatchData.SelectedQuestion = netQuestion;
            SendToPlayersService.SendSelectedQuestion(MatchData.SelectedQuestion);

            MatchData.SelectedRoundQuestion = netRoundQuestion;
            SendToPlayersService.SendSelectedRoundQuestion(MatchData.SelectedRoundQuestion);

            MatchData.CurrentStoryDotIndex.Value = 0;
            SendToPlayersService.SendCurrentStoryDotIndex(MatchData.CurrentStoryDotIndex.Value);
            
            MatchData.Phase.Value = MatchPhase.Question;
            SendToPlayersService.Send(MatchData.Phase.Value);
        }
        
        public void BackToRound()
        {
            SelectRound(MatchData.RoundsInfo.Value.CurrentRoundNumber );
        }
        
        public void SelectRound(int number)
        {
            Assert.IsTrue(NetworkData.IsAdmin);

            MatchData.RoundsInfo.Value.RoundsAmount = PackageData.Package.Rounds.Count;
            MatchData.RoundsInfo.Value.CurrentRoundNumber = number;
            SendToPlayersService.Send(MatchData.RoundsInfo.Value);

            Round round = PackageData.Package.Rounds[number - 1];
            UpdateRoundData(MatchData.RoundData.Value, round);
            SendToPlayersService.Send(MatchData.RoundData.Value);

            MatchData.Phase.Value = MatchPhase.Round;
            SendToPlayersService.Send(MatchData.Phase.Value);
        }

        private void UpdateRoundData(NetRound netRound, Round round)
        {
            netRound.Themes.Clear();
            foreach (Theme theme in round.Themes)
            {
                NetRoundTheme netRoundTheme = new NetRoundTheme();
                netRoundTheme.Name = theme.Name;
                foreach (Question question in theme.Questions)
                {
                    NetRoundQuestion netRoundQuestion = new NetRoundQuestion(question.Id);
                    netRoundQuestion.Price = question.Price;
                    netRoundTheme.Questions.Add(netRoundQuestion);
                }
                netRound.Themes.Add(netRoundTheme);
            }
        }
        
        public void ShowNext()
        {
            if (MatchData.CurrentStoryDot == MatchData.SelectedQuestion.QuestionStory.Last())
            {
                MatchData.Phase.Value = MatchPhase.Answer;
                SendToPlayersService.Send(MatchData.Phase.Value);
            }
            else
            {
                MatchData.CurrentStoryDotIndex.Value++;
                SendToPlayersService.SendCurrentStoryDotIndex(MatchData.CurrentStoryDotIndex.Value);
            }
        }
    }
}