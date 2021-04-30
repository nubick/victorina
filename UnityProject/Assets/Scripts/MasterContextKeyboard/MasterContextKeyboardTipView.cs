using System.Text;
using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class MasterContextKeyboardTipView : ViewBase
    {
        [Inject] private MasterContextKeyboardSystem MasterContextKeyboardSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }

        public Text TipText;

        public void Initialize()
        {
            //MatchData.Phase.SubscribeChanged(RefreshUI);
            //QuestionAnswerData.Phase.SubscribeChanged(RefreshUI);
        }
        
        public void RefreshUI()
        {
            Content.SetActive(true);
            
            StringBuilder sb = new StringBuilder();
            foreach (ContextCommand command in MasterContextKeyboardSystem.Commands)
            {
                if (command.Condition())
                    sb.AppendLine(command.Tip);
            }

            TipText.text = sb.ToString();
        }
    }
}