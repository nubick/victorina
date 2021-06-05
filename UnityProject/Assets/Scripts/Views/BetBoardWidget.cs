using UnityEngine;
using UnityEngine.UI;
using System;

namespace Victorina
{
    public class BetBoardWidget : MonoBehaviour
    {
        private int _minBet;
        private int _maxBet;
        private int _roughBet;
        
        public CanvasGroup BetCanvasGroup;
        public Button AllInButton;
        public Button PassButton;
        public Text RoughBetText;

        public Action<int> MakeBetEvent;
        public Action AllInEvent;
        public Action PassEvent;

        public void Bind(int minBet, int maxBet, int initBet)
        {
            _minBet = minBet;
            _maxBet = maxBet;
            SetRoughBet(initBet);
        }

        public void SetSettings(bool isInteractable, bool isAllInButtonActive, bool isPassButtonActive)
        {
            BetCanvasGroup.interactable = isInteractable;
            AllInButton.interactable = isAllInButtonActive;
            PassButton.interactable = isPassButtonActive;
        }
        
        public void OnMakeBetButtonClicked()
        {
            MakeBetEvent?.Invoke(_roughBet);
        }

        public void OnAllInButtonClicked()
        {
            AllInEvent?.Invoke();
        }

        public void OnPassButtonClicked()
        {
            PassEvent?.Invoke();
        }
        
        public void OnChangeBetButtonClicked(int changeValue)
        {
            SetRoughBet(_roughBet + changeValue);
        }

        private void SetRoughBet(int bet)
        {
            _roughBet = Mathf.Clamp(bet, _minBet, _maxBet);
            RoughBetText.text = $"Поставить\n{_roughBet}";
        }
    }
}