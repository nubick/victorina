using Injection;
using UnityEngine;

namespace Victorina
{
    public class MasterShowFinalRoundQuestionView : ViewBase
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        
        public GameObject PreviousStoryDotButton;
        public GameObject NextStoryDotButton;
        
        private ShowFinalRoundQuestionPlayState PlayState => PlayStateData.As<ShowFinalRoundQuestionPlayState>();
        
        protected override void OnShown()
        {
            RefreshUI();            
        }
        
        public void RefreshUI()
        {
            PreviousStoryDotButton.SetActive(PlayState.StoryDotIndex > 0);
            NextStoryDotButton.SetActive(!PlayState.IsLastDot);
        }
        
        public void OnPreviousStoryDotButtonClicked()
        {
            PlayState.StoryDotIndex--;
        }
        
        public void OnNextStoryDotButtonClicked()
        {
            PlayState.StoryDotIndex++;
        }
        
        public void OnAcceptAnswerButtonClicked()
        {
            FinalRoundSystem.SwitchToAnsweringPhase();
        }
    }
}