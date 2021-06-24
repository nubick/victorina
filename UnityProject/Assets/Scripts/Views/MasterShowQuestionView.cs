using Injection;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Victorina
{
    public class MasterShowQuestionView : ViewBase
    {
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private QuestionAnswerData Data { get; set; }

        [Inject] private PackagePlayStateData PlayStateData { get; set; }

        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();

        public GameObject PreviousQuestionDotButton;
        public GameObject NextQuestionDotButton;
        public GameObject StartTimerButton;
        public GameObject StopTimerButton;
        public GameObject RestartMediaButton;
        public GameObject AcceptAnswer;
        public GameObject ShowAnswerButton;
        public GameObject ShowRoundButton;

        public Image TimerStrip;

        public GameObject AnswerTipPanel;
        public Text AnswerTip;

        public Text ThemeText;
        
        public void Initialize()
        {
            MetagameEvents.TimerRunOut.Subscribe(OnTimerRunOut);
        }
        
        protected override void OnShown()
        {
            RefreshUI();            
        }
        
        public void RefreshUI()
        {
            PreviousQuestionDotButton.SetActive(PlayState.StoryDotIndex > 0);
            NextQuestionDotButton.SetActive(!PlayState.IsLastDot);
            
            TimerStrip.gameObject.SetActive(Data.TimerState != QuestionTimerState.NotStarted);
            RestartMediaButton.SetActive(false);
            StartTimerButton.SetActive(CanStartTimer(Data.TimerState, PlayState.IsLastDot)); 
            StopTimerButton.SetActive(Data.TimerState == QuestionTimerState.Running);
            
            AcceptAnswer.SetActive(true);
            ShowAnswerButton.SetActive(QuestionAnswerSystem.CanShowAnswer());
            ShowRoundButton.SetActive(QuestionAnswerSystem.CanBackToRound());

            AnswerTip.text = Data.AnswerTip;
            AnswerTipPanel.SetActive(Data.IsAnswerTipEnabled);

            ThemeText.text = $"Тема: {PlayState.NetQuestion.GetTheme()}";
        }
        
        private bool CanStartTimer(QuestionTimerState timerState, bool isLastDot)
        {
            if (timerState == QuestionTimerState.Running)
                return false;

            return isLastDot || timerState != QuestionTimerState.NotStarted;
        }
        
        public void Update()
        {
            if (IsActive && Data.TimerState == QuestionTimerState.Running)
            {
                TimerStrip.fillAmount = QuestionTimer.GetLeftSecondsPercentage();
            }
        }

        private void OnTimerRunOut()
        {
            StartTimerButton.SetActive(false);
            StopTimerButton.SetActive(false);
            RestartMediaButton.SetActive(true);
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

        public void OnAcceptAnswerButtonClicked()
        {
            QuestionAnswerSystem.AcceptNoRiskAnswer();
        }

        public void OnToggleTipTriggerClicked()
        {
            Data.IsAnswerTipEnabled = !Data.IsAnswerTipEnabled;
            RefreshUI();
        }
    }
}