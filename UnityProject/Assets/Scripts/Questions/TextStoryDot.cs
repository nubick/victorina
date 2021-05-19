namespace Victorina
{
    public class TextStoryDot : StoryDot
    {
        public string Text { get; }

        public override bool IsMain => true;

        public TextStoryDot(string text)
        {
            Text = text;
        }
    }
}