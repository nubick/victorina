using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class RoundQuestionWidget : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Text Price;
        public Image Background;
        public Color DefaultColor;
        public Color HighlightedColor;
     
        public NetRoundQuestion NetRoundQuestion { get; private set; }

        private bool IsEmpty => NetRoundQuestion == null;
        
        public void Bind(NetRoundQuestion netRoundQuestion)
        {
            NetRoundQuestion = netRoundQuestion;
            Price.text = netRoundQuestion.IsAnswered ? string.Empty : netRoundQuestion.Price.ToString();
        }

        public void BindEmpty()
        {
            NetRoundQuestion = null;
            Price.text = string.Empty;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsEmpty || NetRoundQuestion.IsAnswered)
                return;
            
            MetagameEvents.RoundQuestionClicked.Publish(NetRoundQuestion);
        }

        public void OnEnable()
        {
            ShowDefault();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsEmpty || NetRoundQuestion.IsAnswered)
                return;
            
            ShowHighlighted();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsEmpty || NetRoundQuestion.IsAnswered)
                return;
            
            ShowDefault();
        }

        public void ShowDefault()
        {
            Background.color = DefaultColor;
        }

        public void ShowHighlighted()
        {
            Background.color = HighlightedColor;
        }
    }
}