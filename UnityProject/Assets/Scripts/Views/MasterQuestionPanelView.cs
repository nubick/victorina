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
        
        [Header("Bottom panel")]
        public GameObject PreviousQuestionDotButton;
        public GameObject NextQuestionDotButton;
        public GameObject StartTimerButton;
        public GameObject StopTimerButton;
        public GameObject ShowAnswerButton;
        public GameObject ShowRoundButton;

        public Image TimerStrip;

        [Header("Accept answer panel")]
        public GameObject AcceptAnswerPanel;
        public Text Header;
        public Text AnswerTip;
        
        protected override void OnShown()
        {
            RefreshUI();            
        }
        
        public void RefreshUI()
        {
            QuestionPhase phase = Data.Phase.Value;

            bool canNavigateToPreviousQuestionDot = Data.CurrentStoryDotIndex.Value > 0;
            PreviousQuestionDotButton.SetActive(canNavigateToPreviousQuestionDot);

            bool isLastDot = Data.CurrentStoryDot == Data.CurrentStory.Last();
            NextQuestionDotButton.SetActive(!isLastDot);

            StartTimerButton.SetActive(phase == QuestionPhase.ShowQuestion && (isLastDot || Data.WasTimerStarted) && !Data.IsTimerOn);
            StopTimerButton.SetActive(Data.IsTimerOn);

            ShowAnswerButton.SetActive(phase == QuestionPhase.ShowQuestion && Data.WasTimerStarted);

            ShowRoundButton.SetActive(phase == QuestionPhase.ShowAnswer && isLastDot);
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
            QuestionAnswerSystem.ShowPrevious();
        }
        
        public void OnNextQuestionDotButtonClicked()
        {
            QuestionAnswerSystem.ShowNext();
        }

        public void OnStartTimerButtonClicked()
        {
            QuestionAnswerSystem.StartTimer();
            RefreshUI();
        }

        public void OnStopTimerButtonClicked()
        {
            QuestionAnswerSystem.StopTimer();
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
        
        #region Accept answer panel
        
        private void RefreshAcceptAnswerPanel()
        {
            string playerName = "nubick";
            Header.text = $"Отвечает: {playerName}";
            string answer = "Ты чё, тупой! Этож бубльгум!";
            AnswerTip.text = $"Ответ: \n{answer}";
        }
        
        public void OnCorrectButtonClicked()
        {
            
        }

        public void OnWrongButtonClicked()
        {
            
        }

        public void OnCancelButtonClicked()
        {
            
        }
        
        #endregion
    }
}