using System.Collections;
using UnityEngine.UI;

namespace Victorina
{
    public class InputDialogueView : ViewBase
    {
        public Text Title;
        public InputField InputField;

        public bool IsOk { get; set; }
        public string Text => InputField.text;

        public void SetDefault(string title, string currentString)
        {
            IsOk = false;
            Title.text = title;
            InputField.text = currentString;
        }

        public IEnumerator ShowAndWaitForFinish(string title, string currentString)
        {
            SetDefault(title, currentString);
            return ShowAndWaitForFinish();
        }

        public void OnOkButtonClicked()
        {
            IsOk = true;
            Hide();
        }
    }
}