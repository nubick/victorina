namespace Victorina
{
    public class NetRoundsInfo
    {
        public int RoundsAmount { get; set; }
        public int CurrentRoundNumber { get; set; }
        public RoundType[] RoundTypes { get; set; }

        public override string ToString()
        {
            return $"{nameof(RoundsAmount)}: {RoundsAmount}, {nameof(CurrentRoundNumber)}: {CurrentRoundNumber}";
        }
    }
}