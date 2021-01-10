using System;
using UnityEngine;

namespace Victorina
{
    public class Question
    {
        public string Id { get; }
        public int Price { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }

        public bool IsImage { get; set; }
        public string ImagePath { get; set; }
        public Sprite Image { get; set; }

        public Question() : this(Guid.NewGuid().ToString())
        {
        }

        public Question(string id)
        {
            Id = id;
        }
        
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Price)}: {Price}, {nameof(Text)}: {Text.Substring(20)}";
        }
    }
}