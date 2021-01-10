using Injection;
using Victorina.SI;

namespace Victorina
{
    public class PackageSystem
    {
        [Inject] private PackageData Data { get; set; }

        public void Initialize()
        {
            SiConverter siConverter = new SiConverter();
            Data.Package = siConverter.Convert();;
        }
    }
}