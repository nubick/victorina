using Injection;
using UnityEngine;

namespace Victorina
{
    public class MasterShowAnswerView : ViewBase
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private ShowAnswerSystem ShowAnswerSystem { get; set; }

        public GameObject PreviousQuestionDotButton;
        public GameObject NextQuestionDotButton;
        public GameObject ShowRoundButton;
        
        private ShowAnswerPlayState PlayState => PlayStateData.As<ShowAnswerPlayState>();

        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            PreviousQuestionDotButton.SetActive(PlayState.StoryDotIndex > 0);
            NextQuestionDotButton.SetActive(!PlayState.IsLastDot);
            ShowRoundButton.SetActive(PlayState.IsLastDot);
        }
        
        public void OnPreviousQuestionDotButtonClicked()
        {
            ShowAnswerSystem.ShowPrevious();
        }
        
        public void OnNextQuestionDotButtonClicked()
        {
            ShowAnswerSystem.ShowNext();
        }
        
        public void OnShowRoundButtonClicked()
        {
            ShowAnswerSystem.BackToRound();
        }
    }
}