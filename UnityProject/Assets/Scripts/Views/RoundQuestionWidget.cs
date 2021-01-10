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
        
        public void Bind(NetRoundQuestion netRoundQuestion)
        {
            NetRoundQuestion = netRoundQuestion;
            Price.text = netRoundQuestion.Price.ToString();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            MetagameEvents.RoundQuestionClicked.Publish(NetRoundQuestion);
        }

        public void OnEnable()
        {
            ShowDefault();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowHighlighted();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
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