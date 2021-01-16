using System;
using System.Collections.Generic;

namespace Victorina
{
    public class Question
    {
        public string Id { get; }
        public int Price { get; set; }
        public string Answer { get; set; }

        public List<StoryDot> QuestionStory { get; set; } = new List<StoryDot>();
        public List<StoryDot> AnswerStory { get; set; } = new List<StoryDot>();
        
        public Question() : this(Guid.NewGuid().ToString())
        {
        }

        public Question(string id)
        {
            Id = id;
        }
        
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Price)}: {Price}";
        }
    }
}