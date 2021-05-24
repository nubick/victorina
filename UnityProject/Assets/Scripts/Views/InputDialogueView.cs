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
        
        public IEnumerator ShowAndWaitForFinish(string title, string currentString)
        {
            IsOk = false;
            Title.text = title;
            InputField.text = currentString;
            return ShowAndWaitForFinish();
        }

        public void OnOkButtonClicked()
        {
            IsOk = true;
            Hide();
        }

        public void OnCancelButtonClicked()
        {
            Hide();
        }
    }
}