using Assets.Scripts.Data;
using UnityEngine;

namespace Victorina
{
    public class CatInBagData : MonoBehaviour
    {
        public SoundEffect MeowIntro;
        public SoundEffect MeowAngry;

        public ReactiveProperty<bool> IsPlayerSelected { get; set; } = new ReactiveProperty<bool>();

        public override string ToString()
        {
            return $"IsPlayerSelected: {IsPlayerSelected.Value}";
        }
    }
}