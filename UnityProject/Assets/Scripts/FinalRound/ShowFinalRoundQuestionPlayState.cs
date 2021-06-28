using Victorina;

namespace Victorina
{
    public class ShowFinalRoundQuestionPlayState : StoryDotPlayState
    {
        public FinalRoundPlayState FinalRoundPlayState { get; set; }//Don't sync, server only
        
        public override PlayStateType Type => PlayStateType.ShowFinalRoundQuestion;
        protected override bool IsQuestionStory => true;
    }
}