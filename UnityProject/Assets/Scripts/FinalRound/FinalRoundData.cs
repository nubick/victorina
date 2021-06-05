namespace Victorina
{
    public class FinalRoundData : SyncData
    {
        public string[] Themes { get; private set; }
        public bool[] RemovedThemes { get; set; }

        //Master Only, Don't Sync
        public Round Round { get; set; }
        
        public FinalRoundData()
        {
            Themes = new string[0];
            RemovedThemes = new bool[0];
        }
        
        public FinalRoundData(string[] themes, bool[] removedThemes)
        {
            Themes = themes;
            RemovedThemes = removedThemes;
        }
        
        public void Reset(string[] themes)
        {
            Themes = themes;
            RemovedThemes = new bool[themes.Length];
            MarkAsChanged();
        }

        public void Update(FinalRoundData finalRoundData)
        {
            Themes = finalRoundData.Themes;
            RemovedThemes = finalRoundData.RemovedThemes;
            MarkAsChanged();
        }

        public void RemoveTheme(int index)
        {
            RemovedThemes[index] = true;
            MarkAsChanged();
        }
    }
}