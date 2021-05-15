using System.Collections.Generic;

namespace Victorina
{
    public class SiqLoadedPackageData
    {
        public List<string> PackagesNames { get; } = new List<string>();
        public List<string> PackagesPaths { get; } = new List<string>();
    }
}