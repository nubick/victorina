using System;

namespace Victorina
{
    public class PlayStateData : SyncData
    {
        public PackagePlayState PlayState { get; set; }
        public bool IsLockedForMasterOnly { get; set; }
        public PlayStateType Type => PlayState.Type;

        public override bool HasChanges => base.HasChanges || PlayState is {HasChanges: true};

        public override void ApplyChanges()
        {
            base.ApplyChanges();
            PlayState?.ApplyChanges();
        }

        public void Update(PlayStateData data)
        {
            PlayState = data.PlayState;
        }

        public T As<T>() where T : PackagePlayState
        {
            T castedPlayState = PlayState as T;
            if (castedPlayState == null)
                throw new Exception($"Try to cast PlaySte to type {typeof(T)} when PlayState type is {Type}");
            return castedPlayState;
        }

        public override string ToString()
        {
            return $"{PlayState}";
        }
    }
}