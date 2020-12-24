namespace Victorina
{
    public class MatchData
    {
        public MatchPhase Phase { get; set; }
        public TextQuestion TextQuestion { get; set; }
        
        public override string ToString()
        {
            return $"{nameof(Phase)}: {Phase}";
        }
    }
}