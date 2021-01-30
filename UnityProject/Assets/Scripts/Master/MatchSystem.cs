using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class MatchSystem
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnsweringData QuestionAnsweringData { get; set; }
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        
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
            QuestionAnsweringData.WasTimerStarted = false;
            QuestionAnsweringData.IsTimerOn = false;
            
            QuestionAnsweringData.SelectedQuestion.Value = BuildNetQuestion(netRoundQuestion);
            SendToPlayersService.SendSelectedQuestion(QuestionAnsweringData.SelectedQuestion.Value);

            MatchData.SelectedRoundQuestion = netRoundQuestion;
            SendToPlayersService.SendSelectedRoundQuestion(MatchData.SelectedRoundQuestion);

            QuestionAnsweringData.Phase.Value = QuestionPhase.ShowQuestion;
            SendToPlayersService.Send(MatchData.Phase.Value);

            QuestionAnsweringData.CurrentStoryDotIndex.Value = 0;
            SendToPlayersService.SendCurrentStoryDotIndex(QuestionAnsweringData.CurrentStoryDotIndex.Value);
            
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
            QuestionAnsweringData.CurrentStoryDotIndex.Value++;
            SendToPlayersService.SendCurrentStoryDotIndex(QuestionAnsweringData.CurrentStoryDotIndex.Value);
            StartTimerIfTime();
        }

        private void StartTimerIfTime()
        {
            if (QuestionAnsweringData.WasTimerStarted)
                return;
            
            if (QuestionAnsweringData.Phase.Value == QuestionPhase.ShowQuestion &&
                QuestionAnsweringData.CurrentStoryDot == QuestionAnsweringData.SelectedQuestion.Value.QuestionStory.Last())
                StartTimer();
        }

        public void ShowPrevious()
        {
            QuestionAnsweringData.CurrentStoryDotIndex.Value--;
            SendToPlayersService.SendCurrentStoryDotIndex(QuestionAnsweringData.CurrentStoryDotIndex.Value);
        }

        public void StartTimer()
        {
            QuestionAnsweringData.WasTimerStarted = true;
            QuestionAnsweringData.IsTimerOn = true;
            
            QuestionTimer.Start();
            SendToPlayersService.SendStartTimer(Static.TimeForAnswer, QuestionTimer.LeftSeconds);
            
            QuestionAnsweringData.PlayersButtonClickData.Value.Players.Clear();
            QuestionAnsweringData.PlayersButtonClickData.NotifyChanged();
            SendToPlayersService.SendPlayersButtonClickData(QuestionAnsweringData.PlayersButtonClickData.Value);
        }

        public void StopTimer()
        {
            QuestionTimer.Stop();
            
            QuestionAnsweringData.IsTimerOn = false;
            SendToPlayersService.SendStopTimer();
        }
        
        public void ShowAnswer()
        {
            StopTimer();
            QuestionAnsweringData.Phase.Value = QuestionPhase.ShowAnswer;
            SendToPlayersService.Send(MatchData.Phase.Value);

            QuestionAnsweringData.CurrentStoryDotIndex.Value = 0;
            SendToPlayersService.SendCurrentStoryDotIndex(QuestionAnsweringData.CurrentStoryDotIndex.Value);
        }

        public void OnPlayerButtonClickReceived(ulong playerId, float thoughtSeconds)
        {
            PlayerButtonClickData data = PlayersButtonClickData.Players.SingleOrDefault(_ => _.PlayerId == playerId);
            if (data == null)
            {
                data = new PlayerButtonClickData();
                PlayersButtonClickData.Players.Add(data);
            }

            data.PlayerId = playerId;
            data.Name = ConnectedPlayersData.PlayersIdNameMap[playerId];
            data.Time = thoughtSeconds;

            StopTimer();
            
            QuestionAnsweringData.PlayersButtonClickData.Value = PlayersButtonClickData;
            SendToPlayersService.SendPlayersButtonClickData(PlayersButtonClickData);
        }
    }
}