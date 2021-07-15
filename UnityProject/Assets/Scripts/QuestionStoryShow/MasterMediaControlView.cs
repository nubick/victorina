using Injection;
using UnityEngine;

namespace Victorina
{
    public class MasterMediaControlView : ViewBase
    {
        [Inject] private ShowQuestionSystem ShowQuestionSystem { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }
        
        public GameObject PlayButton;
        public GameObject PauseButton;

        public void Initialize()
        {
            MetagameEvents.AnswerTimerDataChanged.Subscribe(RefreshUI);
        }
        
        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (IsActive)
            {
                PlayButton.SetActive(AnswerTimerData.State == QuestionTimerState.NotStarted || AnswerTimerData.State == QuestionTimerState.Paused);
                PauseButton.SetActive(AnswerTimerData.State == QuestionTimerState.Running);
            }
        }
        
        public void OnPlayButtonClicked()
        {
            ShowQuestionSystem.PlayMedia();
            RefreshUI();
        }

        public void OnPauseButtonClicked()
        {
            ShowQuestionSystem.PauseMedia();
            RefreshUI();
        }

        public void OnRestartButtonClicked()
        {
            ShowQuestionSystem.RestartMedia();
            RefreshUI();
        }
    }
}