namespace Victorina
{
    public abstract class StoryDot
    {
        public virtual bool IsMain => false;
        public abstract char ToLetter();
    }
}