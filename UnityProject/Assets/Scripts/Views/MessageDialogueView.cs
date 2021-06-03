using System.Collections;
using UnityEngine.UI;

namespace Victorina
{
    public class MessageDialogueView : ViewBase
    {
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
    }
}