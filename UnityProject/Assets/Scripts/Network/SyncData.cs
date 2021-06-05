using UnityEngine;

namespace Victorina
{
    public abstract class SyncData
    {
        public bool HasChanges { get; private set; }

        public void MarkAsChanged()
        {
            Debug.Log($"Mark As Changed: {GetType()}");
            HasChanges = true;
        }
        
        public void ApplyChanges()
        {
            HasChanges = false;
        }
    }
}