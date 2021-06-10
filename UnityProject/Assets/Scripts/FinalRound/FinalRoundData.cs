using System.Linq;

namespace Victorina
{
    public class FinalRoundData : SyncData
    {
        public FinalRoundPhase Phase { get; private set; }
        
        public string[] Themes { get; private set; }
        public bool[] RemovedThemes { get; set; }
        
        public bool[] DoneBets { get; private set; }
        public bool IsAllBetsDone => DoneBets.All(isDone => isDone);
        public int RemainedThemesAmount => RemovedThemes.Count(isRemoved => !isRemoved);
        public bool IsAllThemesRemoved => RemainedThemesAmount == 1;
        
        //Master Only, Don't Sync
        public Round Round { get; set; }
        public PlayerData SelectedPlayerByMaster { get; private set; }
        public int[] Bets { get; set; }
        
        public FinalRoundData()
        {
            Themes = new string[0];
            RemovedThemes = new bool[0];
            DoneBets = new bool[0];
            Bets = new int[0];
        }

        public FinalRoundData(FinalRoundPhase phase, string[] themes, bool[] removedThemes, bool[] doneBets)
        {
            Phase = phase;
            Themes = themes;
            RemovedThemes = removedThemes;
            DoneBets = doneBets;
            Bets = new int[0];
        }

        public void Reset(string[] themes)
        {
            Themes = themes;
            RemovedThemes = new bool[themes.Length];
            MarkAsChanged();
        }

        public void SetPhase(FinalRoundPhase phase)
        {
            Phase = phase;
            MarkAsChanged();
        }
        
        public void Update(FinalRoundData finalRoundData)
        {
            Phase = finalRoundData.Phase;
            Themes = finalRoundData.Themes;
            RemovedThemes = finalRoundData.RemovedThemes;
            DoneBets = finalRoundData.DoneBets;
            MarkAsChanged();
        }

        public void RemoveTheme(int index)
        {
            RemovedThemes[index] = true;
            MarkAsChanged();
        }

        public void ClearBets(int playersAmount)
        {
            Bets = new int[playersAmount];
            MarkAsChanged();
        }

        public void SetDoneBets(bool[] doneBets)
        {
            DoneBets = doneBets;
            MarkAsChanged();
        }
        
        public void SetBet(int index, int bet)
        {
            Bets[index] = bet;
            DoneBets[index] = true;
            MarkAsChanged();
        }

        public void SelectPlayer(PlayerData player)
        {
            SelectedPlayerByMaster = player;
            MarkAsChanged();
        }
        
        public override string ToString()
        {
            return $"{nameof(Phase)}: {Phase}, {nameof(Themes)}: {Themes.Length}, {nameof(RemovedThemes)}: {RemovedThemes.Length}, [{nameof(Bets)}: {string.Join(",", Bets)}]";
        }
    }
}