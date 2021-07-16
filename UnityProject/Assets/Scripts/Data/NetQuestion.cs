namespace Victorina
{
    public class NetQuestion
    {
        public string QuestionId { get; set; }
        public QuestionType Type { get; set; }
        public string Theme { get; set; }
        public int Price { get; set; }
        
        public int QuestionStoryDotsAmount { get; set; }
        public StoryDot[] QuestionStory { get; set; }
        
        public int AnswerStoryDotsAmount { get; set; }
        public StoryDot[] AnswerStory { get; set; }

        public CatInBagInfo CatInBagInfo { get; set; }

        public string GetTheme()
        {
            return Type == QuestionType.CatInBag ? CatInBagInfo.Theme : Theme;
        }

        public override string ToString()
        {
            return $"[NetQuestion, Type:{Type}, Q:{QuestionStoryDotsAmount}, A:{AnswerStoryDotsAmount}]";
        }
    }
}