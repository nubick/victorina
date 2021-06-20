using System;
using System.Collections.Generic;

namespace Victorina
{
    public class PackagePlayStateData : SyncData
    {
        public Dictionary<Type, PackagePlayState> PlayStatesMap { get; } = new Dictionary<Type, PackagePlayState>();
        public PackagePlayState PlayState { get; set; }
        public PlayStateType Type => PlayState.Type;
        
        public void Update(PackagePlayStateData data)
        {
            PlayState = data.PlayState;
        }
        
        public override string ToString()
        {
            return $"{PlayState}";
        }
    }
}