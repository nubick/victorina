namespace Victorina
{
    public class NetRoundQuestion
    {
        public string QuestionId { get; }
        public int Price { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }
        public bool IsAnswered { get; set; }
        
        public NetRoundQuestion(string questionId)
        {
            QuestionId = questionId;
        }

        public override string ToString()
        {
            return $"{nameof(QuestionId)}: {QuestionId}, {nameof(IsAnswered)}: {IsAnswered}";
        }
    }
}