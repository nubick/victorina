namespace Victorina
{
    public class ShowAnswerPlayState : StoryDotPlayState
    {
        public override PlayStateType Type => PlayStateType.ShowAnswer;
        protected override bool IsQuestionStory => false;
        public override string ToString() => $"[ShowAnswerPlayState, index: {StoryDotIndex}, question: {NetQuestion}]";
    }
}