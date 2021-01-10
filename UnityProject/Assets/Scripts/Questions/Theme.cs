using System;
using System.Collections.Generic;

namespace Victorina
{
    public class Theme
    {
        public string Id { get; }
        public string Name { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();

        public Theme() : this(Guid.NewGuid().ToString())
        {
        }
        
        public Theme(string id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
        }
    }
}