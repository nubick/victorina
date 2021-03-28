namespace Victorina
{
    public class NetRoundQuestion
    {
        public string QuestionId { get; }
        public int Price { get; set; }
        public bool IsAnswered { get; set; }
        public QuestionType Type { get; set; }
        public string Theme { get; set; }
        
        public bool IsDownloadedByAll { get; set; }
        public int[] FileIds { get; set; }
        
        //Local value for master and player, updated locally
        public bool IsDownloadedByMe { get; set; }
        
        public NetRoundQuestion(string questionId)
        {
            QuestionId = questionId;
        }

        public override string ToString()
        {
            return $"{nameof(QuestionId)}: {QuestionId}, {nameof(Price)}: {Price}";
        }
    }
}