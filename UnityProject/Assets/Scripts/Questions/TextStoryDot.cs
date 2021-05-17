namespace Victorina
{
    public class TextStoryDot : StoryDot
    {
        public string Text { get; }
        
        public TextStoryDot(string text)
        {
            Text = text;
        }
    }
}