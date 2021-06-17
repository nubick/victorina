using System;
using System.Collections;
using UnityEngine.UI;

namespace Victorina
{
    public class ConfirmationDialogueView : ViewBase
    {
        public bool IsConfirmed { get; private set; }

        public Text Title;
        public Text Message;

        public IEnumerator ShowAndWaitForFinish(string title, string message)
        {
            Title.text = title;
            Message.text = message;
            return ShowAndWaitForFinish();
        }

        public void Show(string title, string message)
        {
            Title.text = title;
            Message.text = message;
            Show();
        }

        protected override void OnShown()
        {
            IsConfirmed = false;
        }

        public void OnYesButtonClicked()
        {
            IsConfirmed = true;
            Hide();
        }

        public void OnNoButtonClicked()
        {
            Hide();
        }
    }
}