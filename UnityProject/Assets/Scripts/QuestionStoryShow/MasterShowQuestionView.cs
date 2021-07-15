using Injection;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Victorina
{
    public class MasterShowQuestionView : ViewBase
    {
        [Inject] private QuestionStripTimer QuestionStripTimer { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private ShowQuestionSystem ShowQuestionSystem { get; set; }
        [Inject] private MasterAnswerTipData TipData { get; set; }
        
        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();

        public GameObject PreviousQuestionDotButton;
        public GameObject NextQuestionDotButton;
        public GameObject StartTimerButton;
        public GameObject StopTimerButton;
        public GameObject AcceptAnswer;
        public GameObject ShowAnswerButton;

        public Image TimerStrip;

        public GameObject AnswerTipPanel;
        public Text AnswerTip;

        public Text ThemeText;
        
        public void Initialize()
        {
            MetagameEvents.TimerRunOut.Subscribe(OnTimerRunOut);
            MetagameEvents.AnswerTimerDataChanged.Subscribe(RefreshUI);
        }
        
        protected override void OnShown()
        {
            RefreshUI();            
        }
        
        public void RefreshUI()
        {
            if (!IsActive)
                return;
            
            PreviousQuestionDotButton.SetActive(PlayState.StoryDotIndex > 0);
            NextQuestionDotButton.SetActive(!PlayState.IsLastDot);
            
            TimerStrip.gameObject.SetActive(AnswerTimerData.State != QuestionTimerState.NotStarted);
            StartTimerButton.SetActive(CanStartTimer(AnswerTimerData.State, PlayState.IsLastDot)); 
            StopTimerButton.SetActive(AnswerTimerData.State == QuestionTimerState.Running);
            
            AcceptAnswer.SetActive(PlayState.NetQuestion.Type != QuestionType.Simple && AnswerTimerData.State != QuestionTimerState.NotStarted);
            
            ShowAnswerButton.SetActive(ShowQuestionSystem.CanShowAnswer());

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
            if (IsActive && AnswerTimerData.State == QuestionTimerState.Running)
            {
                TimerStrip.fillAmount = QuestionStripTimer.GetLeftSecondsPercentage();
            }
        }

        private void OnTimerRunOut()
        {
            StartTimerButton.SetActive(false);
            StopTimerButton.SetActive(false);
        }

        public void OnPreviousQuestionDotButtonClicked()
        {
            ShowQuestionSystem.ShowPrevious();
        }
        
        public void OnNextQuestionDotButtonClicked()
        {
            ShowQuestionSystem.ShowNext();
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
        
        public void OnAcceptAnswerButtonClicked()
        {
            ShowQuestionSystem.AcceptSinglePlayerQuestion();
        }

        public void OnToggleTipTriggerClicked()
        {
            TipData.IsAnswerTipEnabled = !TipData.IsAnswerTipEnabled;
            RefreshUI();
        }
    }
}