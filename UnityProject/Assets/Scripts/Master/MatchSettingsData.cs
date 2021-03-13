namespace Victorina
{
    public class MatchSettingsData
    {
        public bool IsLimitAnsweringSeconds { get; set; }
        public float MaxAnsweringSeconds { get; set; }

        public MatchSettingsData()
        {
            IsLimitAnsweringSeconds = false;
            MaxAnsweringSeconds = 5f;
        }
    }
}