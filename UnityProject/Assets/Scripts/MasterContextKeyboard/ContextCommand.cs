using System;
using UnityEngine;

namespace Victorina
{
    public class ContextCommand
    {
        public string Title { get; }
        public Func<bool> Condition { get; set; }
        public KeyCode KeyCode { get; set; }
        public Action Action { get; set; }
        public string Tip { get; set; }
        
        public ContextCommand(string title)
        {
            Title = title;
        }
    }
}