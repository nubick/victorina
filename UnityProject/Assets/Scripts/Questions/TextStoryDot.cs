namespace Victorina
{
    public class TextStoryDot : StoryDot
    {
        public override StoryDotType Type => StoryDotType.Text;
        public string Text { get; }
        
        public TextStoryDot(string text)
        {
            Text = text;
        }
    }
}