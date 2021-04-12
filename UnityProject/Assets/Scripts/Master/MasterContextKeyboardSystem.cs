using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class MasterContextKeyboardSystem : IKeyPressedHandler
    {
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        
        public void OnKeyPressed(KeyCode keyCode)
        {
            if (NetworkData.IsClient)
                return;

            if (MatchData.Phase.Value != MatchPhase.Question)
                return;
            
            Debug.Log($"OnKeyPressed: {keyCode}");
            
            if (QuestionAnswerSystem.CanShowNext())
            {
                if (keyCode == KeyCode.Space || keyCode == KeyCode.RightArrow)
                {
                    Debug.Log($"Master keyboard: {keyCode}. Show next.");
                    QuestionAnswerSystem.ShowNext();
                    return;
                }
            }

            if (QuestionAnswerSystem.CanShowPrevious())
            {
                if (keyCode == KeyCode.LeftArrow)
                {
                    Debug.Log($"Master keyboard: {keyCode}. Show previous.");
                    QuestionAnswerSystem.ShowPrevious();
                    return;
                }
            }

            if (QuestionAnswerSystem.CanShowAnswer())
            {
                if (keyCode == KeyCode.Space || keyCode == KeyCode.RightArrow)
                {
                    if (QuestionAnswerData.PlayersButtonClickData.Players.Any())
                    {
                        Debug.Log($"Master keyboard:{keyCode}. Select fastest player for answer.");
                        QuestionAnswerSystem.SelectFastestPlayerForAnswer();
                    }
                    else
                    {
                        Debug.Log($"Master keyboard:{keyCode}. Show answer.");
                        QuestionAnswerSystem.ShowAnswer();
                    }
                 
                    return;
                }
            }

            if (QuestionAnswerSystem.CanBackToRound())
            {
                if (keyCode == KeyCode.Space || keyCode == KeyCode.RightArrow)
                {
                    Debug.Log($"Master keyboard:{keyCode}. Back to round.");
                    QuestionAnswerSystem.BackToRound();
                    return;
                }
            }
            
        }
    }
}