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
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private ShowQuestionSystem ShowQuestionSystem { get; set; }
        [Inject] private MasterAnswerTipData TipData { get; set; }
        
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
            
            TimerStrip.gameObject.SetActive(AnswerTimerData.TimerState != QuestionTimerState.NotStarted);
            RestartMediaButton.SetActive(false);
            StartTimerButton.SetActive(CanStartTimer(AnswerTimerData.TimerState, PlayState.IsLastDot)); 
            StopTimerButton.SetActive(AnswerTimerData.TimerState == QuestionTimerState.Running);
            
            AcceptAnswer.SetActive(true);
            ShowAnswerButton.SetActive(ShowQuestionSystem.CanShowAnswer());
            ShowRoundButton.SetActive(QuestionAnswerSystem.CanBackToRound());

            AnswerTip.text = TipData.AnswerTip;
            AnswerTipPanel.SetActive(TipData.IsAnswerTipEnabled);

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
            if (IsActive && AnswerTimerData.TimerState == QuestionTimerState.Running)
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
            ShowQuestionSystem.ShowPrevious();
        }
        
        public void OnNextQuestionDotButtonClicked()
        {
            ShowQuestionSystem.ShowNext();
        }

        public void OnRestartMediaButtonClicked()
        {
            ShowQuestionSystem.RestartMedia();
            RefreshUI();
        }
        
        public void OnStartTimerButtonClicked()
        {
            ShowQuestionSystem.ContinueTimer();
            RefreshUI();
        }

        public void OnStopTimerButtonClicked()
        {
            ShowQuestionSystem.PauseTimer();
            RefreshUI();
        }
        
        public void OnShowAnswerButtonClicked()
        {
            ShowQuestionSystem.ShowAnswer();
        }

        public void OnShowRoundButtonClicked()
        {
            QuestionAnswerSystem.BackToRound();
        }

        public void OnAcceptAnswerButtonClicked()
        {
            ShowQuestionSystem.AcceptNoRiskAnswer();
        }

        public void OnToggleTipTriggerClicked()
        {
            TipData.IsAnswerTipEnabled = !TipData.IsAnswerTipEnabled;
            RefreshUI();
        }
    }
}