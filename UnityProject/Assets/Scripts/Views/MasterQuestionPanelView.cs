using System.Linq;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class MasterQuestionPanelView : ViewBase
    {
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private QuestionAnswerData Data { get; set; }
        
        public GameObject PreviousQuestionDotButton;
        public GameObject NextQuestionDotButton;
        public GameObject StartTimerButton;
        public GameObject StopTimerButton;
        public GameObject RestartMediaButton;
        public GameObject ShowAnswerButton;
        public GameObject ShowRoundButton;

        public Image TimerStrip;
        
        protected override void OnShown()
        {
            RefreshUI();            
        }
        
        public void RefreshUI()
        {
            bool canNavigateToPreviousQuestionDot = Data.CurrentStoryDotIndex > 0;
            PreviousQuestionDotButton.SetActive(canNavigateToPreviousQuestionDot);

            bool isLastDot = Data.CurrentStoryDot == Data.CurrentStory.Last();
            NextQuestionDotButton.SetActive(!isLastDot);

            RestartMediaButton.SetActive(false);
            StartTimerButton.SetActive(CanStartTimer(Data.Phase.Value, Data.TimerState, isLastDot)); 
            StopTimerButton.SetActive(Data.TimerState == QuestionTimerState.Running);

            ShowAnswerButton.SetActive(Data.Phase.Value == QuestionPhase.ShowQuestion && Data.TimerState != QuestionTimerState.NotStarted);

            ShowRoundButton.SetActive(Data.Phase.Value == QuestionPhase.ShowAnswer && isLastDot);
        }

        private bool CanStartTimer(QuestionPhase phase, QuestionTimerState timerState, bool isLastDot)
        {
            if (phase != QuestionPhase.ShowQuestion)
                return false;

            if (timerState == QuestionTimerState.Running)
                return false;

            return isLastDot || timerState != QuestionTimerState.NotStarted;
        }
        
        public void Update()
        {
            if (IsActive && Data.TimerState == QuestionTimerState.Running)
            {
                float leftSecondsPercentage = QuestionTimer.GetLeftSecondsPercentage();
                TimerStrip.fillAmount = QuestionTimer.GetLeftSecondsPercentage();
                bool isRunOutOfTime = Mathf.Approximately(leftSecondsPercentage, 0f);

                if (isRunOutOfTime)
                {
                    if (StartTimerButton.activeSelf)
                        StartTimerButton.SetActive(false);

                    if (StopTimerButton.activeSelf)
                        StopTimerButton.SetActive(false);

                    if (!RestartMediaButton.activeSelf)
                        RestartMediaButton.SetActive(true);
                }
            }
        }

        public void OnPreviousQuestionDotButtonClicked()
        {
            QuestionAnswerSystem.ShowPrevious();
        }
        
        public void OnNextQuestionDotButtonClicked()
        {
            QuestionAnswerSystem.ShowNext();
        }

        public void OnRestartMediaButtonClicked()
        {
            QuestionAnswerSystem.RestartMedia();
            RefreshUI();
        }
        
        public void OnStartTimerButtonClicked()
        {
            QuestionAnswerSystem.ContinueTimer();
            RefreshUI();
        }

        public void OnStopTimerButtonClicked()
        {
            QuestionAnswerSystem.PauseTimer();
            RefreshUI();
        }
        
        public void OnShowAnswerButtonClicked()
        {
            QuestionAnswerSystem.ShowAnswer();
        }

        public void OnShowRoundButtonClicked()
        {
            QuestionAnswerSystem.BackToRound();
        }
    }
}