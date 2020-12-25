using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    [RequireComponent(typeof(InputField))]
    public class ValidatedInputField : MonoBehaviour
    {
        public Color ValidColor;
        public Color InvalidColor;
        
        public InputField InputField { get; private set; }

        public string Text
        {
            get => InputField.text;
            set => InputField.text = value;
        }

        public void Awake()
        {
            InputField = GetComponent<InputField>();
            InputField.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnEnable()
        {
            Reset();
        }

        private void OnValueChanged(string newValue)
        {
            Reset();
        }
        
        public void MarkInvalid()
        {
            ColorBlock colors = InputField.colors;
            colors.normalColor = InvalidColor;
            InputField.colors = colors;
        }

        public void Reset()
        {
            ColorBlock colors = InputField.colors;
            colors.normalColor = ValidColor;
            InputField.colors = colors;
        }
    }
}