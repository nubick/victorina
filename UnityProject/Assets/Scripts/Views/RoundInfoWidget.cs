using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class RoundInfoWidget : MonoBehaviour, IPointerClickHandler
    {
        private int _number;
        
        public Image Icon;
        public Text Title;

        public Color PassedColor;
        public Color CurrentColor;
        public Color NextColor;
        
        public void Bind(string title, int number, RoundProgress roundProgress)
        {
            Title.text = title;
            _number = number;
            Icon.color = GetColor(roundProgress);
        }

        private Color GetColor(RoundProgress roundProgress)
        {
            switch (roundProgress)
            {
                case RoundProgress.Passed:
                    return PassedColor;
                case RoundProgress.Current:
                    return CurrentColor;
                case RoundProgress.Next:
                    return NextColor;
                default:
                    throw new Exception($"Not supported round progress: {roundProgress}");
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            MetagameEvents.RoundInfoClicked.Publish(_number);
        }
    }

    public enum RoundProgress
    {
        Passed,
        Current,
        Next
    }
}