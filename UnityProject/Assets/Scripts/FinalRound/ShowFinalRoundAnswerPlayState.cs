namespace Victorina
{
    public class ShowFinalRoundAnswerPlayState : StoryDotPlayState
    {
        public FinalRoundPlayState FinalRoundPlayState { get; set; }//Don't sync, server only
        
        public override PlayStateType Type => PlayStateType.ShowFinalRoundAnswer;
        protected override bool IsQuestionStory => false;
    }
}