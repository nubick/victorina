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

        public override char ToLetter()
        {
            return 'T';
        }
    }
}