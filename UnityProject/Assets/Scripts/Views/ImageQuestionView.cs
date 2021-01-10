using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class ImageQuestionView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private RoundView RoundView { get; set; }

        public Image Image;

        protected override void OnShown()
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(MatchData.SelectedQuestion.ImageBytes);
            Image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        
        public void OnAnswerButtonClicked()
        {
            MatchSystem.BackToRound();
            SwitchTo(RoundView);
        }
    }
}