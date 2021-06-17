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
        
        public bool[] DoneAnswers { get; private set; }
        
        //Accepting
        public string AcceptingInfo { get; private set; }           //Sync with players
        public int AcceptingPlayerIndex { get; set; }               //Don't sync
        public FinalRoundAcceptingPhase AcceptingPhase { get; set; }//Don't sync
        public bool IsAcceptedAsCorrect { get; set; }               //Don't sync
        
        //Master Only, Don't Sync
        public Round Round { get; set; }
        public PlayerData SelectedPlayerByMaster { get; private set; }
        public int[] Bets { get; set; }
        public string[] Answers { get; set; }

        public FinalRoundData()
        {
            Themes = new string[0];
            RemovedThemes = new bool[0];
            DoneBets = new bool[0];
            Bets = new int[0];
            Answers = new string[0];
            DoneAnswers = new bool[0];
            AcceptingInfo = string.Empty;
        }

        public FinalRoundData(FinalRoundPhase phase, string[] themes, bool[] removedThemes)
        {
            Phase = phase;
            Themes = themes;
            RemovedThemes = removedThemes;
            Bets = new int[0];
            DoneBets = new bool[0];
            Answers = new string[0];
            DoneAnswers = new bool[0];
            AcceptingInfo = string.Empty;
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
            DoneAnswers = finalRoundData.DoneAnswers;
            AcceptingInfo = finalRoundData.AcceptingInfo;
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

        public void ClearAnswers(int playersAmount)
        {
            Answers = new string[playersAmount];
            MarkAsChanged();
        }

        public void SetDoneAnswers(bool[] doneAnswer)
        {
            DoneAnswers = doneAnswer;
            MarkAsChanged();
        }

        public void SetAnswer(int index, string answerText)
        {
            Answers[index] = answerText;
            DoneAnswers[index] = true;
            MarkAsChanged();
        }

        public void ClearAnswer(int index)
        {
            Answers[index] = null;
            DoneAnswers[index] = false;
            MarkAsChanged();
        }

        public void SetAcceptingInfo(string acceptingInfo)
        {
            AcceptingInfo = acceptingInfo;
            MarkAsChanged();
        }
        
        public override string ToString()
        {
            return $"{nameof(Phase)}: {Phase}, {nameof(Themes)}: {Themes.Length}, {nameof(RemovedThemes)}: {RemovedThemes.Length}, [{nameof(Bets)}: {string.Join(",", Bets)}]";
        }
    }
}