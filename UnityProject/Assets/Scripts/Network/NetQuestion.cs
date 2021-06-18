using System;

namespace Victorina
{
    public class NetQuestion
    {
        public QuestionType Type { get; set; }
        
        public int QuestionStoryDotsAmount { get; set; }
        public StoryDot[] QuestionStory { get; set; }
        
        public int AnswerStoryDotsAmount { get; set; }
        public StoryDot[] AnswerStory { get; set; }

        public CatInBagInfo CatInBagInfo { get; set; }
        
        public T GetFirst<T>() where T : StoryDot
        {
            foreach (StoryDot storyDot in QuestionStory)
                if (storyDot is T firstStoryDot)
                    return firstStoryDot;

            foreach (StoryDot storyDot in AnswerStory)
                if (storyDot is T firstStoryDot)
                    return firstStoryDot;

            throw new Exception($"Can't find story dot of type '{typeof(T)}'. QuestionType: {Type}");
        }

        public override string ToString()
        {
            return $"[NQ, Type:{Type}, Q:{QuestionStoryDotsAmount}, A:{AnswerStoryDotsAmount}]";
        }
    }
}