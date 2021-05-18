using System;
using System.Collections.Generic;
using System.Linq;

namespace Victorina
{
    public class Question
    {
        public string Id { get; }
        public int Price { get; set; }
        public QuestionType Type { get; set; }

        public List<StoryDot> QuestionStory { get; } = new List<StoryDot>();
        public List<StoryDot> AnswerStory { get; } = new List<StoryDot>();

        public Question() : this(Guid.NewGuid().ToString())
        {
        }

        public Question(string id)
        {
            Id = id;
            Type = QuestionType.Simple;
        }

        public List<StoryDot> GetAllStories()
        {
            List<StoryDot> allStories = new List<StoryDot>();
            allStories.AddRange(QuestionStory);
            allStories.AddRange(AnswerStory);
            return allStories;
        }

        public List<StoryDot> GetAllMainStories()
        {
            //todo: remove this when special questions be refactored as not story dots.
            List<StoryDot> allStories = GetAllStories().Where(
                storyDot => storyDot is TextStoryDot ||
                            storyDot is ImageStoryDot ||
                            storyDot is AudioStoryDot ||
                            storyDot is VideoStoryDot).ToList();
            return allStories;
        }
        
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Price)}: {Price}";
        }
    }
}