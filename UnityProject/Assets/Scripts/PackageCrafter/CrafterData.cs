using System.Collections.Generic;

namespace Victorina
{
    public class CrafterData
    {
        public List<Package> Packages { get; set; } = new List<Package>();
        public Package SelectedPackage { get; set; }
        public Round SelectedRound { get; set; }
        public Theme SelectedTheme { get; set; }
        public Question SelectedQuestion { get; set; }
        
        public int PreviewStoryDotIndex { get; set; }
        
        public List<Theme> BagAllThemes { get; set; } = new List<Theme>();
        public List<Theme> BagSelectedThemes { get; set; } = new List<Theme>();
    }
}