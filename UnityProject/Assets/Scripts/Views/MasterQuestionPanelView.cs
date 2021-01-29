using System;
using System.Linq;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class MasterQuestionPanelView : ViewBase
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        
        public GameObject PreviousQuestionDotButton;
        public GameObject NextQuestionDotButton;
        public GameObject StartTimerButton;
        public GameObject StopTimerButton;
        public GameObject ShowAnswerButton;
        public GameObject ShowRoundButton;

        public Image TimerStrip;
        
        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            MatchPhase phase = MatchData.Phase.Value;
            
            bool canNavigateToPreviousQuestionDot = MatchData.CurrentStoryDotIndex.Value > 0;
            PreviousQuestionDotButton.SetActive(canNavigateToPreviousQuestionDot);
            
            StoryDot[] story = GetCurrentStory(MatchData.SelectedQuestion.Value, phase);
            bool isLastDot = MatchData.CurrentStoryDot == story.Last();
            NextQuestionDotButton.SetActive(!isLastDot);
            
            StartTimerButton.SetActive(phase == MatchPhase.ShowQuestion && (isLastDot || MatchData.WasTimerStarted ) && !MatchData.IsTimerOn);
            StopTimerButton.SetActive(MatchData.IsTimerOn);

            ShowAnswerButton.SetActive(phase == MatchPhase.ShowQuestion && MatchData.WasTimerStarted);
            
            ShowRoundButton.SetActive(phase == MatchPhase.ShowAnswer && isLastDot);
        }

        private StoryDot[] GetCurrentStory(NetQuestion netQuestion, MatchPhase phase)
        {
            if (phase == MatchPhase.ShowQuestion)
                return netQuestion.QuestionStory;
            
            if (phase == MatchPhase.ShowAnswer)
                return netQuestion.AnswerStory;

            throw new Exception($"Can't get current story for phase: {phase}");
        }
        
        public void Update()
        {
            if (IsActive)
            {
                TimerStrip.fillAmount = QuestionTimer.GetLeftSecondsPercentage();
            }
        }

        public void OnPreviousQuestionDotButtonClicked()
        {
            MatchSystem.ShowPrevious();
        }
        
        public void OnNextQuestionDotButtonClicked()
        {
            MatchSystem.ShowNext();
            RefreshUI();
        }

        public void OnStartTimerButtonClicked()
        {
            MatchSystem.StartTimer();
            RefreshUI();
        }

        public void OnStopTimerButtonClicked()
        {
            MatchSystem.StopTimer();
            RefreshUI();
        }
        
        public void OnShowAnswerButtonClicked()
        {
            MatchSystem.ShowAnswer();
        }

        public void OnShowRoundButtonClicked()
        {
            MatchSystem.BackToRound();
        }
    }
}