using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class ShowAnswerSystem
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        private ShowAnswerPlayState PlayState => PlayStateData.As<ShowAnswerPlayState>();

        public void ShowNext()
        {
            PlayState.StoryDotIndex++;
            PlayStateData.MarkAsChanged();
        }

        public void ShowPrevious()
        {
            PlayState.StoryDotIndex--;
            PlayStateData.MarkAsChanged();
        }
        
        public void BackToRound()
        {
            CommandsSystem.AddNewCommand(new FinishQuestionCommand());
        }
    }
}