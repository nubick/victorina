using System.Collections;
using UnityEngine.UI;

namespace Victorina
{
    public class RoundAutoPriceInputView : ViewBase
    {
        public InputField BasePriceInputField;
        public InputField PriceStepInputField;

        public bool IsOk { get; set; }

        public void SetDefault(int basePrice, int priceStep)
        {
            IsOk = false;
            BasePriceInputField.text = basePrice.ToString();
            PriceStepInputField.text = priceStep.ToString();
        }
        
        public IEnumerator ShowAndWaitForFinish(int basePrice, int priceStep)
        {
            SetDefault(basePrice, priceStep);
            return ShowAndWaitForFinish();
        }
        
        public void OnFillButtonClicked()
        {
            IsOk = true;
            Hide();
        }
    }
}