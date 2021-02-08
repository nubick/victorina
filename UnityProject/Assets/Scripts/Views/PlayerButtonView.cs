using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerButtonView : ViewBase
    {
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private PlayerAnswerSystem PlayerAnswerSystem { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }

        public Image TimerStrip;
        public GameObject AnswerButton;
        public GameObject AnsweringPanel;
        public Text AnsweringText;
        public GameObject AnswerPanel;
        public GameObject WasWrongAnswerPanel;
        
        public void Initialize()
        {
            QuestionAnswerData.Phase.SubscribeChanged(RefreshUI);
        }

        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            QuestionPhase phase = QuestionAnswerData.Phase.Value;
            AnswerButton.SetActive(PlayerAnswerSystem.CanSendAnswerIntention());
            WasWrongAnswerPanel.SetActive(phase == QuestionPhase.ShowQuestion && PlayerAnswerSystem.WasWrongAnswer());
            AnsweringPanel.SetActive(phase == QuestionPhase.AcceptingAnswer);
            AnsweringText.text = $"Отвечает: {QuestionAnswerData.AnsweringPlayerName}";
            AnswerPanel.SetActive(phase == QuestionPhase.ShowAnswer);
        }

        public void OnAnswerButtonClicked()
        {
            PlayerAnswerSystem.OnAnswerButtonClicked();
        }

        public void Update()
        {
            if (IsActive)
            {
                float leftSeconds = QuestionTimer.GetLeftSecondsPercentage();
                TimerStrip.fillAmount = leftSeconds;
            }
        }
    }
}