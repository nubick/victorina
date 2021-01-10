using Injection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Victorina
{
    public class MatchSystem
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        
        public void Start()
        {
            SelectRound(PackageData.Package.Rounds[0]);
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
            MatchData.SelectedQuestion = netRoundQuestion;
            SendToPlayersService.SendSelectedQuestion(MatchData.SelectedQuestion);

            MatchData.Phase.Value = MatchPhase.Question;
            SendToPlayersService.Send(MatchData.Phase.Value);
        }

        public void BackToRound()
        {
            SelectRound(PackageData.Package.Rounds[0]);
        }

        public void SelectRound(Round round)
        {
            Assert.IsTrue(NetworkData.IsAdmin);
            
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
                    netRoundQuestion.Text = question.Text;
                    netRoundQuestion.Answer = question.Answer;
                    netRoundTheme.Questions.Add(netRoundQuestion);
                }
                netRound.Themes.Add(netRoundTheme);
            }
        }
    }
}