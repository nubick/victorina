namespace Victorina
{
    public class NoRiskStoryDot : StoryDot
    {
        public override string ToString()
        {
            return "[nrsd]";
        }

        public override char ToLetter()
        {
            return 'R';
        }
    }
}