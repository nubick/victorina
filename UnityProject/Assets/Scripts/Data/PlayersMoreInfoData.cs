namespace Victorina
{
    public class PlayersMoreInfoData
    {
        public string[] InfoTexts { get; private set; }
        public bool[] Highlights { get; private set; }
        public bool[] Selections { get; private set; }

        public void Update(string[] infoTexts, bool[] highlights, bool[] selections)
        {
            InfoTexts = infoTexts;
            Highlights = highlights;
            Selections = selections;
            MetagameEvents.PlayersMoreInfoDataChanged.Publish();
        }

        public int Size => InfoTexts.Length;
    }
}