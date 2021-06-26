using System;
using System.Collections.Generic;

namespace Victorina
{
    public class PackagePlayStateData : SyncData
    {
        public PackagePlayState PlayState { get; set; }
        public PlayStateType Type => PlayState.Type;

        public void Update(PackagePlayStateData data)
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