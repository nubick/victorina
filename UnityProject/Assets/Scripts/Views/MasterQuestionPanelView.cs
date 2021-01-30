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

        private void RefreshUI()
        {
            RefreshUI(MatchData.QuestionAnsweringData);
        }

        private void RefreshUI(QuestionAnsweringData data)
        {
            QuestionPhase phase = data.Phase.Value;

            bool canNavigateToPreviousQuestionDot = data.CurrentStoryDotIndex.Value > 0;
            PreviousQuestionDotButton.SetActive(canNavigateToPreviousQuestionDot);

            bool isLastDot = data.CurrentStoryDot == data.CurrentStory.Last();
            NextQuestionDotButton.SetActive(!isLastDot);

            StartTimerButton.SetActive(phase == QuestionPhase.ShowQuestion && (isLastDot || data.WasTimerStarted) && !data.IsTimerOn);
            StopTimerButton.SetActive(data.IsTimerOn);

            ShowAnswerButton.SetActive(phase == QuestionPhase.ShowQuestion && data.WasTimerStarted);

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