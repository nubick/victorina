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
        [Inject] private CatInBagData CatInBagData { get; set; }
        
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
        
        private StoryDot PreviousStoryDot => Data.CurrentStory[Data.CurrentStoryDotIndex - 1];
        
        public void Initialize()
        {
            MetagameEvents.TimerRunOut.Subscribe(OnTimerRunOut);
            CatInBagData.IsPlayerSelected.SubscribeChanged(RefreshUI);
        }
        
        protected override void OnShown()
        {
            RefreshUI();            
        }
        
        public void RefreshUI()
        {
            QuestionType questionType = Data.SelectedQuestion.Value.Type;
            QuestionPhase phase = Data.Phase.Value;

            bool canNavigateToPreviousQuestionDot = Data.CurrentStoryDotIndex > 0 && !(PreviousStoryDot is CatInBagStoryDot) && !(PreviousStoryDot is NoRiskStoryDot);
            PreviousQuestionDotButton.SetActive(canNavigateToPreviousQuestionDot);

            bool isLastDot = Data.CurrentStoryDot == Data.CurrentStory.Last();
            bool isWaitingWhoGetCatInBag = Data.CurrentStoryDot is CatInBagStoryDot && !CatInBagData.IsPlayerSelected.Value;
            NextQuestionDotButton.SetActive(!isLastDot && !isWaitingWhoGetCatInBag);
            
            TimerStrip.gameObject.SetActive(!isWaitingWhoGetCatInBag);

            RestartMediaButton.SetActive(false);
            StartTimerButton.SetActive(CanStartTimer(phase, Data.TimerState, isLastDot)); 
            StopTimerButton.SetActive(Data.TimerState == QuestionTimerState.Running);
            
            //only for No Risk and Cat in Bag questions
            if(questionType == QuestionType.Simple || Data.CurrentStoryDot is NoRiskStoryDot || Data.CurrentStoryDot is CatInBagStoryDot)
                AcceptAnswer.SetActive(false);
            else
                AcceptAnswer.SetActive(phase == QuestionPhase.ShowQuestion);
            
            ShowAnswerButton.SetActive(questionType == QuestionType.Simple && phase == QuestionPhase.ShowQuestion && Data.TimerState != QuestionTimerState.NotStarted);

            ShowRoundButton.SetActive(phase == QuestionPhase.ShowAnswer && isLastDot);

            AnswerTip.text = Data.AnswerTip;
            AnswerTipPanel.SetActive(Data.IsAnswerTipEnabled);
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