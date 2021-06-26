namespace Victorina
{
    public abstract class SyncData
    {
        public virtual bool HasChanges { get; private set; }

        public void MarkAsChanged()
        {
            HasChanges = true;
        }
        
        public virtual void ApplyChanges()
        {
            HasChanges = false;
        }
    }
}