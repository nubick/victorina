using System;
using Injection;

namespace Victorina
{
    public abstract class StoryDotView : ViewBase
    {
        [Inject] protected PlayStateData PlayStateData { get; set; }

        protected StoryDot GetCurrentStoryDot()
        {
            if (PlayStateData.PlayState is StoryDotPlayState storyDotPlayState)
                return storyDotPlayState.CurrentStoryDot;
            
            if (PlayStateData.Type == PlayStateType.AcceptingAnswer)
                return PlayStateData.As<AcceptingAnswerPlayState>().ShowQuestionPlayState.CurrentStoryDot;

            throw new Exception($"Not supported PlayState: {PlayStateData}");
        }
    }
}