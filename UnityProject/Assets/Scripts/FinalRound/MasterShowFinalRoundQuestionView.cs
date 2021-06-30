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

        public GameObject AcceptAnswerButton;
        public GameObject BackButton;
        
        private StoryDotPlayState PlayState => PlayStateData.As<StoryDotPlayState>();
        
        protected override void OnShown()
        {
            RefreshUI();            
        }
        
        public void RefreshUI()
        {
            PreviousStoryDotButton.SetActive(PlayState.StoryDotIndex > 0);
            NextStoryDotButton.SetActive(!PlayState.IsLastDot);
            
            AcceptAnswerButton.SetActive(PlayStateData.Type == PlayStateType.ShowFinalRoundQuestion);
            BackButton.SetActive(PlayStateData.Type == PlayStateType.ShowFinalRoundAnswer);
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

        public void OnBackButtonClicked()
        {
            FinalRoundSystem.SwitchBackToAnswerAcceptingPhase();
        }
    }
}