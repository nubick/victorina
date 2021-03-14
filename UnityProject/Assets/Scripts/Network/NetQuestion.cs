
namespace Victorina
{
    public class NetQuestion
    {
        public QuestionType Type { get; set; }
        
        public int QuestionStoryDotsAmount { get; set; }
        public StoryDot[] QuestionStory { get; set; }
        
        public int AnswerStoryDotsAmount { get; set; }
        public StoryDot[] AnswerStory { get; set; }

        public override string ToString()
        {
            return $"[NQ, Type:{Type}, Q:{QuestionStoryDotsAmount}, A:{AnswerStoryDotsAmount}]";
        }
    }
}