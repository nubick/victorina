using Injection;

namespace Victorina
{
    public class VideoStoryDotView : ViewBase
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        
        public void OnNextButtonClicked()
        {
            MatchSystem.ShowNext();
        }
    }
}