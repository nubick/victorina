namespace Victorina
{
    public class Question
    {
        public int Price { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }

        public override string ToString()
        {
            return $"{nameof(Price)}: {Price}, {nameof(Text)}: {Text}, {nameof(Answer)}: {Answer}";
        }
    }
}