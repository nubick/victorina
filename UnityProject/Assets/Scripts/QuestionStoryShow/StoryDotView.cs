using System;
using Injection;

namespace Victorina
{
    public abstract class StoryDotView : ViewBase
    {
        [Inject] protected PackagePlayStateData PlayStateData { get; set; }

        protected StoryDot GetCurrentStoryDot()
        {
            return PlayStateData.Type switch
            {
                PlayStateType.ShowQuestion => PlayStateData.As<ShowQuestionPlayState>().CurrentStoryDot,
                PlayStateType.ShowAnswer => PlayStateData.As<ShowAnswerPlayState>().CurrentStoryDot,
                PlayStateType.AcceptingAnswer => PlayStateData.As<AcceptingAnswerPlayState>().ShowQuestionPlayState.CurrentStoryDot,
                _ => throw new Exception($"Not supported PlayState: {PlayStateData}")
            };
        }
    }
}