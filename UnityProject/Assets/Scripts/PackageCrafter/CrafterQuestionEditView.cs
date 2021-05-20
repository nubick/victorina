using System;
using System.Collections.Generic;
using System.Linq;
using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterQuestionEditView : ViewBase
    {
        [Inject] private CrafterData CrafterData { get; set; }
        [Inject] private PackageCrafterSystem PackageCrafterSystem { get; set; }
        
        public Dropdown TypeDropdown;
        public InputField PriceInputField;
        
        protected override void OnShown()
        {
            List<string> options = Enum.GetNames(typeof(QuestionType)).ToList();
            TypeDropdown.ClearOptions();
            TypeDropdown.AddOptions(options);

            QuestionType type = CrafterData.SelectedQuestion.Type;
            int index = options.IndexOf(type.ToString());
            TypeDropdown.SetValueWithoutNotify(index);
            
            PriceInputField.SetTextWithoutNotify(CrafterData.SelectedQuestion.Price.ToString());
        }

        public void OnOkButtonClicked()
        {
            QuestionType selectedType = (QuestionType) TypeDropdown.value;
            
            int newPrice = CrafterData.SelectedQuestion.Price;
            if (int.TryParse(PriceInputField.text, out int parsedPrice))
                newPrice = parsedPrice;

            PackageCrafterSystem.UpdateSelectedQuestion(selectedType, newPrice);
            
            Hide();
        }
    }
}