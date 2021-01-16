using Injection;

namespace Victorina
{
    public class AudioStoryDotView : ViewBase
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        
        public void OnNextButtonClicked()
        {
            MatchSystem.ShowNext();
        }
    }
}