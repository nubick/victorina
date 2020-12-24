namespace Victorina
{
    public class TextQuestion
    {
        public string Question { get; set; }
        public string Answer { get; set; }

        public override string ToString()
        {
            return $"{nameof(Question)}: {Question}, {nameof(Answer)}: {Answer}";
        }
    }
}