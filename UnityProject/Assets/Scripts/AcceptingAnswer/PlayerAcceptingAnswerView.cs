using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerAcceptingAnswerView : ViewBase
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        
        public Text AnsweringText;

        private AcceptingAnswerPlayState PlayState => PlayStateData.As<AcceptingAnswerPlayState>();
        
        protected override void OnShown()
        {
            AnsweringText.text = $"Отвечает: {PlayersBoardSystem.GetPlayerName(PlayState.AnsweringPlayerId)}";
        }
    }
}