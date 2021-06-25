using System;
using System.Linq;
using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class ShowQuestionSystem
    {
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        
        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();
        private bool IsTimeToStartTimer => AnswerTimerData.State == QuestionTimerState.NotStarted && PlayState.IsLastDot;

        public void Start()
        {
            QuestionTimer.Reset(Static.TimeForAnswer);

            AnswerTimerData.State = QuestionTimerState.NotStarted;
            PlayState.WrongAnsweredIds.Clear();
            ResetAdmittedPlayersIds(PlayState.NetQuestion.Type);
            
            if (IsTimeToStartTimer)
                StartTimer();
        }
        
        private void ResetAdmittedPlayersIds(QuestionType questionType)
        {
            PlayState.AdmittedPlayersIds.Clear();
            switch (questionType)
            {
                case QuestionType.Simple:
                    PlayState.AdmittedPlayersIds.AddRange(PlayersBoard.Players.Select(_ => _.PlayerId));
                    break;
                case QuestionType.NoRisk:
                case QuestionType.CatInBag:
                case QuestionType.Auction:
                    PlayState.AdmittedPlayersIds.Add(PlayersBoard.Current.PlayerId);
                    break;
                default:
                    throw new Exception($"Not supported question type: {questionType}");
            }
        }

        public void ShowNext()
        {
            PlayState.StoryDotIndex++;
            
            if (IsTimeToStartTimer)
                StartTimer();
        }

        public void ShowPrevious()
        {
            PlayState.StoryDotIndex--;
        }

        public void StartTimer()
        {
            AnswerTimerData.ResetSeconds = Static.TimeForAnswer;
            AnswerTimerData.LeftSeconds = QuestionTimer.LeftSeconds;
            AnswerTimerData.State = QuestionTimerState.Running;
            PlayersButtonClickData.Clear();
        }

        public void PauseTimer()
        {
            AnswerTimerData.State = QuestionTimerState.Paused;
        }

        public void ContinueTimer()
        {
            StartTimer();
        }

        public void RestartMedia()
        {
            QuestionTimer.Reset(Static.TimeForAnswer);
            StartTimer();
        }

        public bool CanShowAnswer()
        {
            return PlayState.NetQuestion.Type == QuestionType.Simple && AnswerTimerData.State != QuestionTimerState.NotStarted;
        }

        public void ShowAnswer()
        {
            CommandsSystem.AddNewCommand(new ShowAnswerCommand());
        }
        
        public void SelectFastestPlayerForAnswer()
        {
            if (NetworkData.IsMaster)
                CommandsSystem.AddNewCommand(new SelectFastestPlayerForAnswerCommand());
        }
        
        public void SelectPlayerForAnswer(byte playerId)
        {
            if (NetworkData.IsMaster)
                CommandsSystem.AddNewCommand(new SelectPlayerForAnswerCommand {PlayerId = playerId});
        }

        public void AcceptSinglePlayerQuestion()
        {
            if (PlayersBoard.Current == null)
            {
                Debug.Log("Master. Error. Can't accept no risk answer. Current player is null.");
            }
            else
            {
                PauseTimer();
                SelectPlayerForAnswer(PlayersBoard.Current.PlayerId);
            }
        }
    }
}