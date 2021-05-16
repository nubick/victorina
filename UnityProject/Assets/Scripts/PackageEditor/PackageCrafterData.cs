using System.Collections.Generic;

namespace Victorina
{
    public class PackageCrafterData
    {
        public List<Package> Packages { get; set; } = new List<Package>();
        public Package SelectedPackage { get; set; }
        public Round SelectedRound { get; set; }
        public Theme SelectedTheme { get; set; }
        public Question SelectedQuestion { get; set; }
    }
}