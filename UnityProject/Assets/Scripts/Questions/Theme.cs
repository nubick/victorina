using System.Collections.Generic;

namespace Victorina
{
    public class Theme
    {
        public string Name { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}